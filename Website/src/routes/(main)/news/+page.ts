import type { PageLoad } from './$types';
import { getPageNoFromParams, makeRequestUrl, newsSchema } from './news.ts';

export const load: PageLoad = async ({ fetch, url }) => {
  const pageNo = getPageNoFromParams(url.searchParams);
  const requestUrl = makeRequestUrl(pageNo, url.origin);

  const response = await fetch(requestUrl);
  if (!response.ok) {
    throw new Error(`News API call failed with status ${response.status}`);
  }

  return {
    newsItems: newsSchema.parse(await response.json())
  };
};
