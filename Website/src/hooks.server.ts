import type { Handle, HandleFetch, HandleServerError } from '@sveltejs/kit';

import { env } from '$env/dynamic/private';
import { PUBLIC_ENABLE_MSW } from '$env/static/public';
import Cookies from '$lib/auth/cookies.ts';
import getJwtMetadata from '$lib/auth/jwt.ts';
import createLogger from '$lib/server/logger.ts';

if (!env.DAWNSHARD_API_URL_SSR) {
  throw new Error('Failed to load environment variable DAWNSHARD_API_URL_SSR!');
}

const internalApiUrl = new URL(env.DAWNSHARD_API_URL_SSR);

if (PUBLIC_ENABLE_MSW === 'true') {
  const { server } = await import('./mocks/node');
  server.listen({
    onUnhandledRequest: (request, print) => {
      if (!request.url.includes('baas.lukefz.xyz')) {
        print.warning();
      }
    }
  });
}

export const handleFetch: HandleFetch = ({ request, fetch, event }) => {
  const logger = createLogger('handleFetch');
  const requestUrl = new URL(request.url);

  if (event.url.origin === requestUrl.origin && requestUrl.pathname.startsWith('/api')) {
    // Rewrite URL to internal
    const newUrl = request.url.replace(requestUrl.origin, internalApiUrl.origin);
    logger.debug(
      { oldUrl: requestUrl.href, newUrl },
      'Rewriting request: from {oldUrl} to {newUrl}'
    );

    // We need to explicitly add the JWT back in, because SvelteKit seems to refuse to forward cookies here; it's
    // possible it views the request as changing origins and no longer internal.
    const idToken = event.cookies.get(Cookies.IdToken);
    if (idToken) {
      request.headers.append('Authorization', `Bearer ${idToken}`);
    }

    return fetch(new Request(newUrl, request));
  }

  return fetch(request);
};

export const handle: Handle = ({ event, resolve }) => {
  const idToken = event.cookies.get(Cookies.IdToken);

  if (!idToken) {
    event.locals.hasValidJwt = false;
    return resolve(event);
  }

  const jwtMetadata = getJwtMetadata(idToken);
  const valid = jwtMetadata.valid && jwtMetadata.expiryTimestampMs > Date.now();

  event.locals.hasValidJwt = valid;

  if (!valid) {
    event.cookies.delete(Cookies.IdToken, { path: '/' });
  }

  return resolve(event);
};

export const handleError: HandleServerError = ({ error, status, message }) => {
  const logger = createLogger('handleError');
  logger.error({ error, status, message }, 'Unhandled error occurred: {message}');
};
