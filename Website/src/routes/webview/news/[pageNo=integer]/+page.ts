import { makeRequestUrl, newsSchema } from '$main/news/news.ts';

import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch, params, url }) => {
  const pageNo = Number.parseInt(params.pageNo) || 1;
  const requestUrl = makeRequestUrl(pageNo, url.origin);

  const response = await fetch(requestUrl);

  return {
    news: newsSchema.parse(await response.json())
  };
};
