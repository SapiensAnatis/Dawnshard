import type { PageLoad } from './$types';
import { userProfileSchema } from './userProfile.ts';

export const load: PageLoad = async ({ fetch, url }) => {
  const userRequest = new URL('/api/user/me/profile', url.origin);

  const response = await fetch(userRequest);

  if (!response.ok) {
    throw new Error(`/user/me/profile error: HTTP ${response.status}`);
  }

  return {
    userProfile: userProfileSchema.parse(await response.json())
  };
};
