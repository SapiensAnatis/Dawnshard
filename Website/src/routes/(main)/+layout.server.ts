import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = ({ locals, url }) => {
  return {
    hasValidJwt: locals.hasValidJwt,
    isAdmin: locals.isAdmin,
    urlOrigin: url.origin
  };
};
