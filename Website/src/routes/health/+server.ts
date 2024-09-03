import { env } from '$env/dynamic/private';

import type { RequestHandler } from './$types';

export const GET: RequestHandler = async ({ fetch }) => {
  const apiPingUrl = new URL('/ping', env.DAWNSHARD_API_URL_SSR);

  try {
    const apiPing = await fetch(apiPingUrl);

    if (!apiPing.ok) {
      console.error(`Failed to ping ${apiPingUrl}: status ${apiPing.status}`);
      return new Response('Unhealthy', { status: 503 });
    }
  } catch (e) {
    console.error(`Fetch failed when pinging ${apiPingUrl}`, e);
    return new Response('Unhealthy', { status: 503 });
  }

  return new Response('Healthy', { status: 200 });
};
