import Cookies from '$lib/auth/cookies.ts';

import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = ({ locals, depends }) => {
  depends(`cookie:${Cookies.IdToken}`);

  return {
    hasValidJwt: locals.hasValidJwt
  };
};
