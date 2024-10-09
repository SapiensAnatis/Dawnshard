import { getPageNoFromParams, makeRequestUrl, newsSchema } from '$main/news/news.ts';

import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch, url }) => {
  const pageNo = getPageNoFromParams(url.searchParams) || 1;
  const requestUrl = makeRequestUrl(pageNo, url.origin);

  const response = await fetch(requestUrl);

  return {
    news: newsSchema.parse(await response.json())
  };
};
