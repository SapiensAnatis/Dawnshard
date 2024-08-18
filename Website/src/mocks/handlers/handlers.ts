import {
  type DefaultBodyType,
  http as mswHttp,
  HttpResponse,
  type HttpResponseResolver,
  type PathParams
} from 'msw';

import { handleNews, handleNewsItem } from './news.ts';
import handleSavefileExport from './savefile.ts';
import { handleUser, handleUserProfile } from './user.ts';
import { handlePresentData } from './widgets.ts';

const createHttpHandler = <
  Params extends PathParams<keyof Params> = PathParams,
  RequestBodyType extends DefaultBodyType = DefaultBodyType,
  ResponseBodyType extends DefaultBodyType = DefaultBodyType
>(
  method: keyof typeof mswHttp
) => {
  return (
    path: string,
    resolver: HttpResponseResolver<Params, RequestBodyType, ResponseBodyType>
  ) => [mswHttp[method](path, resolver), mswHttp[method](`http://localhost:5000${path}`, resolver)];
};

const http = {
  all: createHttpHandler('all'),
  head: createHttpHandler('head'),
  get: createHttpHandler('get'),
  post: createHttpHandler('post'),
  put: createHttpHandler('put'),
  delete: createHttpHandler('delete'),
  patch: createHttpHandler('patch'),
  options: createHttpHandler('options')
};

const withAuth = <
  Params extends PathParams,
  RequestBodyType extends DefaultBodyType,
  ResponseBodyType extends DefaultBodyType
>(
  resolver: HttpResponseResolver
):
  | HttpResponseResolver<Params, RequestBodyType, ResponseBodyType>
  | HttpResponseResolver<Params, RequestBodyType, undefined> => {
  return (input) => {
    const { cookies, request } = input;
    if (!cookies['idToken'] && !request.headers.get('Authorization')) {
      return new HttpResponse(null, { status: 401 });
    }

    return resolver(input);
  };
};

export const handlers = [
  ...http.get('/api/news', handleNews),
  ...http.get('/api/news/:itemId', handleNewsItem),

  ...http.get('/api/user/me', withAuth(handleUser)),
  ...http.get('/api/user/me/profile', withAuth(handleUserProfile)),

  ...http.get('/api/savefile/export', withAuth(handleSavefileExport)),
  ...http.get('/api/widgets/present', handlePresentData),
  mswHttp.get('http://localhost:5000/ping', () => new Response(null, { status: 200 }))
];
