import { timeAttackClearArraySchema } from '../timeAttackTypes.ts';
import type { PageLoad } from './$types';

const pageSize = 10;

export const load: PageLoad = async ({ fetch, params, url }) => {
  const pageNumber = Number(url.searchParams.get('page')) || 1;
  const pageIndex = pageNumber - 1;

  const pagingParams = new URLSearchParams({
    offset: (pageIndex * pageSize).toString(),
    pageSize: pageSize.toString()
  });

  const endpoint = `/api/time_attack/rankings/${params.questId}?${pagingParams}`;
  const requestUrl = new URL(endpoint, url.origin);

  const response = await fetch(requestUrl);

  if (!response.ok) {
    throw new Error(`${endpoint} call failed with status ${response.status}`);
  }

  return {
    clearData: timeAttackClearArraySchema.parse(await response.json())
  };
};
