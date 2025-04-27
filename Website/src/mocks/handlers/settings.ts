import { delay, HttpResponse, type HttpResponseResolver } from 'msw';

export const handleSettings: HttpResponseResolver = async () => {
  await delay(500);

  return HttpResponse.text('', { status: 200 });
};
