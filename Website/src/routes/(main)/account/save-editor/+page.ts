import type { PageLoad } from './$types';
import { presentWidgetDataSchema } from './present/presentTypes.ts';

const endpoint = '/api/savefile/edit/widgets/present';

export const load: PageLoad = async ({ fetch, url }) => {
  const presentRequest = new URL(endpoint, url.origin);

  const response = await fetch(presentRequest);

  if (!response.ok) {
    throw new Error(`${endpoint} error: HTTP ${response.status}`);
  }

  return {
    presentWidgetData: presentWidgetDataSchema.parse(await response.json())
  };
};
