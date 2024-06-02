import type { PageLoad } from './$types';
import { makeRequestUrl, newsSchema } from '../news.ts';

export const load: PageLoad = async ({ fetch, params }) => {
  const pageNo = Number.parseInt(params.pageNo) || 1;
  const requestUrl = makeRequestUrl(pageNo);

  const response = await fetch(requestUrl);
  if (!response.ok) {
    throw new Error(`News API call failed with status ${response.status}`);
  }

  return {
    newsItems: newsSchema.parse(await response.json())
  };
};
