import {
  type DefaultBodyType,
  HttpResponse,
  type HttpResponseResolver,
  type PathParams
} from 'msw';

import type {
  TimeAttackRanking,
  TimeAttackRankingResponse
} from '$main/events/time-attack/rankings/timeAttackTypes.ts';

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

export const handleRankings: HttpResponseResolver<
  PathParams,
  DefaultBodyType,
  TimeAttackRankingResponse
> = async ({ request }) => {
  const data = (await getData(request)) as TimeAttackRanking[];

  if (!data) {
    return HttpResponse.json({
      pagination: {
        totalCount: 0
      },
      data: []
    });
  }

  const url = new URL(request.url);

  const offset = Number(url.searchParams.get('offset')) || 0;
  const pageSize = Number(url.searchParams.get('pageSize')) || 10;

  const paginatedData = data.slice(offset, offset + pageSize);

  return HttpResponse.json({
    pagination: {
      totalCount: data.length
    },
    data: paginatedData
  });
};
