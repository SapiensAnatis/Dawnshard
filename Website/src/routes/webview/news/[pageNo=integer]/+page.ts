import type { PageLoad } from './$types';
import { makeRequestUrl, newsSchema } from '$main/news/news.ts';

export const load: PageLoad = async ({ fetch, params }) => {
  const pageNo = Number.parseInt(params.pageNo) || 1;
  const requestUrl = makeRequestUrl(pageNo);

  const newsPromise = fetch(requestUrl)
    .then(async (response) => {
      if (!response.ok) {
        throw new Error(`News API call failed with status ${response.status}`);
      }

      return newsSchema.parse(await response.json());
    })
    .catch((err) => {
      console.error('Failed to load news:', err);
      return null;
    });

  return {
    newsPromise
  };
};
