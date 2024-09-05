import { HttpResponse, type HttpResponseResolver } from 'msw';

import { getData } from './util.ts';

export const handleQuests: HttpResponseResolver = () =>
  HttpResponse.json([
    {
      id: 227010104,
      isCoop: false
    },
    {
      id: 227010105,
      isCoop: true
    }
  ]);

export const handleRankings: HttpResponseResolver = async ({ request }) => {
  const data = await getData(request);

  if (!data) {
    return HttpResponse.json([]);
  }

  return HttpResponse.json(data);
};
