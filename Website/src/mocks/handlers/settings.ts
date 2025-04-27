import {
  type DefaultBodyType,
  delay,
  HttpResponse,
  type HttpResponseResolver,
  type PathParams
} from 'msw';

export const handleSettings: HttpResponseResolver = async () => {
  await delay(500);

  return HttpResponse.text('', { status: 200 });
};
