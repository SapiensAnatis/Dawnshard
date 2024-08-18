import { superValidate } from 'sveltekit-superforms';
import { zod } from 'sveltekit-superforms/adapters';

import type { PageServerLoad } from './$types.js';
import { presentFormSchema } from './presentFormSchema.ts';

export const load: PageServerLoad = async () => {
  return {
    form: await superValidate(zod(presentFormSchema))
  };
};
