import type { PageServerLoad } from './$types';
import Cookies from '$lib/auth/cookies.ts';
import { redirect } from '@sveltejs/kit';

export const load: PageServerLoad = async ({ cookies }) => {
  cookies.delete(Cookies.IdToken, { path: '/' });

  redirect(302, '/');
};
