import { HttpResponse, type HttpResponseResolver } from 'msw';

// For some reason the client-side MSW won't receive any cookies, so we don't do auth checks here.

export const handleUser: HttpResponseResolver = () => {
  return HttpResponse.json({
    viewerId: 1,
    name: 'Euden'
  });
};

export const handleUserProfile: HttpResponseResolver = () => {
  return HttpResponse.json({
    lastSaveImportTime: '2024-06-29T11:57:40Z',
    lastLoginTime: '2024-06-28T11:57:40Z'
  });
};
