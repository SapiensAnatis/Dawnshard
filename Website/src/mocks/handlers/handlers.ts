import {
  type DefaultBodyType,
  http as mswHttp,
  type HttpResponseResolver,
  type PathParams
} from 'msw';

import { handleNews, handleNewsItem } from './news.ts';
import handleSavefileExport from './savefile.ts';
import { handleUser, handleUserProfile } from './user.ts';

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
  head: createHttpHandler('head'),
  get: createHttpHandler('get'),
  post: createHttpHandler('post'),
  put: createHttpHandler('put'),
  delete: createHttpHandler('delete'),
  patch: createHttpHandler('patch'),
  options: createHttpHandler('options')
};

export const handlers = [
  http.get('/api/news', handleNews),
  http.get('/api/news/:itemId', handleNewsItem),

  http.head('/api/user/me', handleUser),
  http.get('/api/user/me', handleUser),
  http.get('/api/user/me/profile', handleUserProfile),

  http.get('/api/savefile/export', handleSavefileExport)
].flatMap((x) => x);
