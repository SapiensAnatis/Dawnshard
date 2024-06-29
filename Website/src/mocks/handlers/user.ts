import {
  HttpResponse,
  type HttpResponseResolver,
  type PathParams,
  type DefaultBodyType,
  type JsonBodyType
} from 'msw';

export const handleUser: HttpResponseResolver<PathParams, DefaultBodyType, undefined> = ({
  request,
  cookies
}) => {
  console.log({ h: request.headers, cookies });
  if (!request.headers.has('Authorization') && !cookies.idToken) {
    console.log('returning 401');
    return new HttpResponse(null, { status: 401 });
  }

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
