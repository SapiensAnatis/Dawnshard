import { http, HttpResponse } from 'msw';

export const handlers = [
  http.get(/.*\/api\/news/, ({ params }) => {
    console.log('intercepted news');

    return HttpResponse.json({
      pagination: {
        totalCount: 1
      },
      data: [
        {
          id: 3,
          headline: 'MockServiceWorker Announces Debut 3',
          description: 'MSW is here!',
          date: '2024-06-28T22:43:49Z',
          headerImageSrc: null,
          bodyImageSrc: null
        }
      ]
    });
  })
];
