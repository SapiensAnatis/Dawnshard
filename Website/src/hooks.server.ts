import { PUBLIC_DAWNSHARD_API_URL } from '$env/static/public';
import { DAWNSHARD_API_URL_SSR } from '$env/static/private';

const publicApiUrl = new URL(PUBLIC_DAWNSHARD_API_URL);
const internalApiUrl = new URL(DAWNSHARD_API_URL_SSR);

export const handleFetch = async ({ request, fetch }) => {
  const requestUrl = new URL(request.url);
  if (requestUrl.origin === publicApiUrl.origin) {
    // Rewrite URL to internal
    const newUrl = request.url.replace(publicApiUrl.origin, internalApiUrl.origin);
    console.log('Rewrote URL', requestUrl.toString(), 'to', newUrl.toString());

    return await fetch(new Request(newUrl, request));
  }

  return await fetch(request);
};
