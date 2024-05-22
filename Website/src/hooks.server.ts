import { PUBLIC_DAWNSHARD_API_URL } from '$env/static/public';
import { DAWNSHARD_API_URL_SSR } from '$env/static/private';

const publicApiUrl = new URL(PUBLIC_DAWNSHARD_API_URL);
const internalApiUrl = new URL(DAWNSHARD_API_URL_SSR);

export const handleFetch = async ({ request, fetch }) => {
  const requestUrl = new URL(request.url);
  console.log(requestUrl);
  if (requestUrl.host === publicApiUrl.host) {
    // Rewrite URL to internal
    const newUrl = new URL(request.url);
    newUrl.host = internalApiUrl.host;
    newUrl.protocol = 'http';

    console.log('Rewrote URL', requestUrl.toString(), 'to', newUrl.toString());

    return await fetch(newUrl);
  }

  return await fetch(requestUrl);
};
