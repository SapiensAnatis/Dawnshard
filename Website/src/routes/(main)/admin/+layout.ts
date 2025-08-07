import { redirect } from '@sveltejs/kit';

import { userSchema } from '$main/account/user.ts';

import type { LayoutLoad } from './$types';

export const load: LayoutLoad = async ({ fetch, url }) => {
  const userRequest = new URL('/api/user/me', url.origin);

  const response = await fetch(userRequest);

  if (!response.ok) {
    if (response.status >= 400 && response.status <= 499) {
      redirect(
        303,
        `/unauthorized/${response.status}?originalPage=${encodeURIComponent(url.pathname)}`
      );
    } else {
      throw new Error(`/user/me error: HTTP ${response.status}`);
    }
  }

  const user = userSchema.parse(await response.json());

  if (!('admin' in user.claims)) {
    redirect(303, `/unauthorized/403?originalPage=${encodeURIComponent(url.pathname)}`);
  }
};
