import { HttpResponse, type HttpResponseResolver } from 'msw';

import type { UserProfile } from '$main/account/profile/userProfile.ts';

export const handleUser: HttpResponseResolver = () => {
  return HttpResponse.json({
    viewerId: 1,
    name: 'Euden'
  });
};

export const handleUserProfile: HttpResponseResolver = () => {
  const settings: UserProfile['settings'] = { dailyGifts: true, useLegacyHelpers: false };

  return HttpResponse.json({
    lastSaveImportTime: '2024-06-29T11:57:40Z',
    lastLoginTime: '2024-06-28T11:57:40Z',
    settings
  });
};
