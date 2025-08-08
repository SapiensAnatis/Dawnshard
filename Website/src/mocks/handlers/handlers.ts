import {
  type DefaultBodyType,
  http,
  HttpResponse,
  type HttpResponseResolver,
  type PathParams
} from 'msw';

import { handleNews, handleNewsItem } from './news.ts';
import { handleSavefileEdit, handleSavefileExport } from './savefile.ts';
import { handleSettings } from './settings.ts';
import { handleQuests, handleRankings } from './timeAttack.ts';
import {
  handleSetUserImpersonation,
  handleUser,
  handleUserImpersonation,
  handleUserProfile
} from './user.ts';
import { handlePresentData } from './widgets.ts';

// We need to be able to intercept requests to the Vite proxy (http://localhost:3001) as well as
// SSR requests to the "API" (http://localhost:5000 or whatever is in .env).
const BASE_URL = 'http://localhost*';

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
  http.get('http://localhost:5000/ping', () => new Response(null, { status: 200 })),

  http.get(`${BASE_URL}/api/news`, handleNews),
  http.get(`${BASE_URL}/api/news/:itemId`, handleNewsItem),

  http.get(`${BASE_URL}/api/user/me`, withAuth(handleUser)),
  http.get(`${BASE_URL}/api/user/me/profile`, withAuth(handleUserProfile)),

  http.put(`${BASE_URL}/api/settings`, withAuth(handleSettings)),

  http.get(`${BASE_URL}/api/savefile/export`, withAuth(handleSavefileExport)),
  http.post(`${BASE_URL}/api/savefile/edit`, withAuth(handleSavefileEdit)),
  http.get(`${BASE_URL}/api/savefile/edit/widgets/present`, withAuth(handlePresentData)),

  http.get(`${BASE_URL}/api/time_attack/quests`, handleQuests),
  http.get(`${BASE_URL}/api/time_attack/rankings/*`, handleRankings),

  http.get(`${BASE_URL}/api/user/impersonation_session`, withAuth(handleUserImpersonation)),
  http.put(`${BASE_URL}/api/user/impersonation_session`, withAuth(handleSetUserImpersonation))
];
