import type { LayoutLoad } from './$types';
import { questListSchema } from './timeAttackTypes.ts';

const endpoint = '/api/time_attack/quests';

export const load: LayoutLoad = async ({ fetch, url }) => {
  const questsRequest = new URL(endpoint, url.origin);

  const response = await fetch(questsRequest);

  if (!response.ok) {
    throw new Error(`${endpoint} error: HTTP ${response.status}`);
  }

  return {
    questList: questListSchema.parse(await response.json())
  };
};
