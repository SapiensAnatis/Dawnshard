import { newsItemSchema } from '$main/news/news.ts';

import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch, params, url }) => {
  const id = Number.parseInt(params.id) || 1;
  const requestUrl = new URL(`/api/news/${id}`, url.origin);

  const response = await fetch(requestUrl);

  if (!response.ok) {
    throw new Error(`News API call failed with status ${response.status}`);
  }

  return {
    newsItem: newsItemSchema.parse(await response.json())
  };
};
