import Cookies from '$lib/auth/cookies.ts';

import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = ({ locals, depends, url }) => {
  depends(`cookie:${Cookies.IdToken}`);

  return {
    hasValidJwt: locals.hasValidJwt,
    urlOrigin: url.origin
  };
};
