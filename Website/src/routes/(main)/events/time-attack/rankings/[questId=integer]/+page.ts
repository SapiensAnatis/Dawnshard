import { timeAttackClearArraySchema } from '../timeAttackTypes.ts';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch, params, url }) => {
  const endpoint = `/api/time_attack/rankings/${params.questId}`;
  const requestUrl = new URL(endpoint, url.origin);

  const response = await fetch(requestUrl);

  if (!response.ok) {
    throw new Error(`${endpoint} call failed with status ${response.status}`);
  }

  return {
    clearData: timeAttackClearArraySchema.parse(await response.json())
  };
};
