import { redirect } from '@sveltejs/kit';

import Cookies from '$lib/auth/cookies.ts';

import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ cookies }) => {
  cookies.delete(Cookies.IdToken, { path: '/' });
  cookies.delete(Cookies.IsAdmin, { path: '/' });

  redirect(302, '/');
};
