import { redirect } from '@sveltejs/kit';

import { PUBLIC_DAWNSHARD_API_URL } from '$env/static/public';
import { userSchema } from '$main/account/user.ts';

import type { LayoutLoad } from './$types';

export const load: LayoutLoad = async ({ fetch }) => {
  const userRequest = new URL('user/me', PUBLIC_DAWNSHARD_API_URL);

  const response = await fetch(userRequest);

  if (!response.ok) {
    if (response.status >= 400 && response.status <= 499) {
      redirect(303, `/unauthorized/${response.status}`);
    } else {
      throw new Error(`/user/me error: HTTP ${response.status}`);
    }
  }

  return {
    user: userSchema.parse(await response.json())
  };
};
