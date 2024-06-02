import { PUBLIC_DAWNSHARD_API_URL } from '$env/static/public';
import type { PageLoad } from './$types';
import { newsItemSchema } from '$main/news/news.ts';

export const load: PageLoad = async ({ fetch, params }) => {
  const id = Number.parseInt(params.id) || 1;
  const requestUrl = new URL(`news/${id}`, PUBLIC_DAWNSHARD_API_URL);

  const response = await fetch(requestUrl);

  if (!response.ok) {
    throw new Error(`News API call failed with status ${response.status}`);
  }

  return {
    newsItem: newsItemSchema.parse(await response.json())
  };
};
