import {
  type DefaultBodyType,
  http as mswHttp,
  HttpResponse,
  type HttpResponseResolver,
  type PathParams
} from 'msw';

import { handleNews, handleNewsItem } from './news.ts';
import { handleSavefileEdit, handleSavefileExport } from './savefile.ts';
import { handleQuests, handleRankings } from './timeAttack.ts';
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
  ) => [mswHttp[method](path, resolver), mswHttp[method](`http://localhost*${path}`, resolver)];
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
  resolver:
    | HttpResponseResolver<Params, RequestBodyType, ResponseBodyType>
    | HttpResponseResolver<Params, RequestBodyType, undefined>
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
  mswHttp.get('http://localhost:5000/ping', () => new Response(null, { status: 200 })),

  ...http.get('/api/news', handleNews),
  ...http.get('/api/news/:itemId', handleNewsItem),

  ...http.get('/api/user/me', withAuth(handleUser)),
  ...http.get('/api/user/me/profile', withAuth(handleUserProfile)),

  ...http.get('/api/savefile/export', withAuth(handleSavefileExport)),
  ...http.post('/api/savefile/edit', withAuth(handleSavefileEdit)),
  ...http.get('/api/savefile/edit/widgets/present', withAuth(handlePresentData)),

  ...http.get('/api/time_attack/quests', handleQuests),
  ...http.get('/api/time_attack/rankings/*', handleRankings)
];
