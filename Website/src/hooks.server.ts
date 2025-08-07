import { randomUUID } from 'node:crypto';

import type { Handle, HandleFetch, HandleServerError } from '@sveltejs/kit';
import { sequence } from '@sveltejs/kit/hooks';
import { generateSetInitialModeExpression } from 'mode-watcher';

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

export const handleFetch: HandleFetch = ({ request, event, fetch }) => {
  const { logger } = event.locals;
  const requestUrl = new URL(request.url);

  logger.debug({ url: requestUrl.href }, 'Sending fetch request to {url}');

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

    // /api/user/me will manually attach the header, avoid adding a second header in this case.
    if (idToken && !request.headers.has('Authorization')) {
      request.headers.append('Authorization', `Bearer ${idToken}`);
    }

    return fetch(new Request(newUrl, request));
  }

  return fetch(request);
};

const handleHeadScript: Handle = ({ event, resolve }) => {
  if (event.request.url.includes('webview')) {
    // Don't inject dark mode script into webview pages, otherwise a user with the storage key set
    // from visiting the actual website will get dark mode in-game, which looks bad
    return resolve(event);
  }

  return resolve(event, {
    transformPageChunk: ({ html }) => {
      return html.replace('%modewatcher.snippet%', generateSetInitialModeExpression({}));
    }
  });
};

const handleLogger: Handle = ({ event, resolve }) => {
  event.locals.logger = createLogger({
    requestPath: new URL(event.request.url).pathname,
    requestId: randomUUID()
  });

  return resolve(event);
};

const handleAuth: Handle = ({ event, resolve }) => {
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
  } else {
    event.locals.logger.fields['jwtSubject'] = jwtMetadata.subject;
  }

  return resolve(event);
};

const handleClaims: Handle = ({ event, resolve }) => {
  event.locals.isAdmin = false;
  const claims = event.cookies.get(Cookies.Claims);

  if (!claims) {
    return resolve(event);
  }

  let claimsObject: unknown;

  try {
    claimsObject = JSON.parse(claims);
  } catch (err) {
    console.error('Failed to parse claim cookie: ', err);
    return resolve(event);
  }

  if (!claimsObject || typeof claimsObject !== 'object') {
    console.error('Cookie was valid JSON but invalid type');
    return resolve(event);
  }

  event.locals.isAdmin = 'admin' in claimsObject;

  return resolve(event);
};

export const handle = sequence(handleHeadScript, handleLogger, handleAuth, handleClaims);

export const handleError: HandleServerError = ({ error, event, status, message }) => {
  event.locals.logger.error({ error, status, message }, 'Unhandled error occurred: {message}');
};
