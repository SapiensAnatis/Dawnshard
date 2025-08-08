import { HttpResponse, type HttpResponseResolver, type PathParams } from 'msw';

import type { UserProfile } from '$main/account/profile/userProfile.ts';

export const handleUser: HttpResponseResolver = () => {
  return HttpResponse.json({
    viewerId: 1,
    name: 'Euden',
    isAdmin: true
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

export const handleUserImpersonation: HttpResponseResolver = () => {
  return HttpResponse.json({
    impersonatedViewerId: 2
  });
};

export const handleSetUserImpersonation: HttpResponseResolver<
  PathParams,
  { target: number | null }
> = async ({ request }) => {
  const data = await request.clone().formData();

  const viewerId = data.get('impersonatedViewerId');
  if (!viewerId || typeof viewerId !== 'string') {
    return HttpResponse.text(undefined, { status: 400 });
  }

  if (viewerId === 'null') {
    // clear
    return HttpResponse.json({
      impersonatedViewerId: null
    });
  }

  const viewerIdNumber = parseInt(viewerId);
  if (!viewerIdNumber) {
    return HttpResponse.text(undefined, { status: 400 });
  }

  return HttpResponse.json({
    impersonatedViewerId: viewerIdNumber
  });
};

export const handleClearUserImpersonation: HttpResponseResolver = () => {
  return new HttpResponse(null, { status: 200 });
};
