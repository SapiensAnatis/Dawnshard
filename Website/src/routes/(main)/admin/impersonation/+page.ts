import { impersonationSessionSchema } from '$main/admin/impersonation/impersonationSession.ts';

import type { PageLoad } from './$types';

const endpoint = '/api/user/me/impersonation_session';

export const load: PageLoad = async ({ fetch, url }) => {
  const presentRequest = new URL(endpoint, url.origin);

  const response = await fetch(presentRequest);

  if (!response.ok) {
    throw new Error(`${endpoint} error: HTTP ${response.status}`);
  }

  return impersonationSessionSchema.parse(await response.json());
};
