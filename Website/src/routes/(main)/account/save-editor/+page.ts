import { superValidate } from 'sveltekit-superforms';
import { zod } from 'sveltekit-superforms/adapters';

import { presentFormSchema } from '$main/account/save-editor/presentFormSchema.ts';

import type { PageLoad } from './$types';
import { presentWidgetDataSchema } from './presentFormSchema.ts';

export const load: PageLoad = async ({ fetch, url }) => {
  const presentRequest = new URL('/api/widgets/present', url.origin);

  const response = await fetch(presentRequest);

  if (!response.ok) {
    throw new Error(`/api/widgets/present error: HTTP ${response.status}`);
  }

  return {
    presentWidgetData: presentWidgetDataSchema.parse(await response.json()),
    form: await superValidate(zod(presentFormSchema))
  };
};
