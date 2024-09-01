import type { LayoutLoad } from './$types';
import { questArraySchema } from './timeAttackTypes.ts';

const endpoint = '/api/time_attack/quests';

export const load: LayoutLoad = async ({ fetch, url }) => {
  const questsRequest = new URL(endpoint, url.origin);

  const response = await fetch(questsRequest);

  if (!response.ok) {
    throw new Error(`${endpoint} error: HTTP ${response.status}`);
  }

  return {
    questList: questArraySchema.parse(await response.json())
  };
};
