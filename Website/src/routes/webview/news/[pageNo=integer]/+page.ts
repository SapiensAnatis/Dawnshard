import { makeRequestUrl, newsSchema } from '$main/news/news.ts';

import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch, params }) => {
  const pageNo = Number.parseInt(params.pageNo) || 1;
  const requestUrl = makeRequestUrl(pageNo);

  const response = await fetch(requestUrl);

  return {
    news: newsSchema.parse(await response.json())
  };
};
