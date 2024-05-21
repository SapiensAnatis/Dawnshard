import type { LayoutLoad } from './$types';
import { PUBLIC_DAWNSHARD_API_URL } from '$env/static/public';
import { userSchema } from './user';

const userUrl = new URL('/api/user', PUBLIC_DAWNSHARD_API_URL);

export const load: LayoutLoad = async ({ fetch }) => {
  const userResponse = await fetch(userUrl);

  if (userResponse.status == 401) {
    return;
  } else if (!userResponse.ok) {
    throw new Error(`${userUrl} request failed: ${userResponse.status}`);
  }

  const userInfo = userSchema.parse(await userResponse.json());

  return { userInfo };
};
