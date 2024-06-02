import { PUBLIC_DAWNSHARD_API_URL } from '$env/static/public';
import { DAWNSHARD_API_URL_SSR } from '$env/static/private';
import type { Handle, HandleFetch } from '@sveltejs/kit';
import Cookies from '$lib/auth/cookies.ts';
import getJwtMetadata from '$lib/auth/jwt.ts';

const publicApiUrl = new URL(PUBLIC_DAWNSHARD_API_URL);
const internalApiUrl = new URL(DAWNSHARD_API_URL_SSR);

export const handleFetch: HandleFetch = ({ request, fetch }) => {
  const requestUrl = new URL(request.url);
  if (requestUrl.origin === publicApiUrl.origin) {
    // Rewrite URL to internal
    const newUrl = request.url.replace(publicApiUrl.origin, internalApiUrl.origin);
    return fetch(new Request(newUrl, request));
  }

  return fetch(request);
};

export const handle: Handle = ({ event, resolve }) => {
  const idToken = event.cookies.get(Cookies.IdToken);
  if (!idToken) {
    event.locals.hasValidJwt = false;
  } else {
    const jwtMetadata = getJwtMetadata(idToken);
    event.locals.hasValidJwt = jwtMetadata.valid && jwtMetadata.expiryTimestampMs > Date.now();
  }

  return resolve(event);
};
