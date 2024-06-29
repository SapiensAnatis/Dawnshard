import { PUBLIC_DAWNSHARD_API_URL } from '$env/static/public';

import type { PageLoad } from './$types';
import { userProfileSchema } from './userProfile.ts';

export const load: PageLoad = async ({ fetch }) => {
  const userRequest = new URL('user/me/profile', PUBLIC_DAWNSHARD_API_URL);

  const response = await fetch(userRequest);

  if (!response.ok) {
    throw new Error(`/user/me/profile error: HTTP ${response.status}`);
  }

  return {
    userProfile: userProfileSchema.parse(await response.json())
  };
};
