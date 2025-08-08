import { HttpResponse, type HttpResponseResolver, type PathParams } from 'msw';

import type { UserProfile } from '$main/account/profile/userProfile.ts';

export const handleUser: HttpResponseResolver = () => {
  return HttpResponse.json({
    viewerId: 1,
    name: 'Euden',
    claims: {
      admin: 'true'
    }
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
  const body = await request.clone().formData();

  const viewerId = body.get('impersonatedViewerId');
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
