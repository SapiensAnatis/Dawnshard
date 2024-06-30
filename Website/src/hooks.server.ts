import type { Handle, HandleFetch } from '@sveltejs/kit';

import { env } from '$env/dynamic/private';
import { PUBLIC_DAWNSHARD_API_URL } from '$env/static/public';
import { PUBLIC_ENABLE_MSW } from '$env/static/public';
import Cookies from '$lib/auth/cookies.ts';
import getJwtMetadata from '$lib/auth/jwt.ts';

const publicApiUrl = new URL(PUBLIC_DAWNSHARD_API_URL);
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

export const handleFetch: HandleFetch = ({ request, fetch }) => {
  const requestUrl = new URL(request.url);

  if (requestUrl.origin === publicApiUrl.origin) {
    // Rewrite URL to internal
    const newUrl = request.url.replace(publicApiUrl.origin, internalApiUrl.origin);
    console.log(`Rewriting request: from ${requestUrl.href} to ${newUrl}`);
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
