import type { Actions } from './$types';

export const actions = {
  settings: async ({ request }) => {
    const data = await request.formData();
    console.log('settings changed', data);

    return { ...Object.fromEntries(data) };
  }
} satisfies Actions;
