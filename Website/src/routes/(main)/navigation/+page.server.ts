import type { PageServerLoad } from './$types';

export const load: PageServerLoad = ({ locals }) => ({
  hasValidJwt: locals.hasValidJwt,
  isAdmin: locals.isAdmin
});
