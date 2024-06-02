import type { LayoutServerLoad } from './$types';
import Cookies from '$lib/auth/cookies.ts';

export const load: LayoutServerLoad = ({ locals, depends }) => {
  depends(`cookie:${Cookies.IdToken}`);

  return {
    hasValidJwt: locals.hasValidJwt
  };
};
