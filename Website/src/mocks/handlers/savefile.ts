import {
  type DefaultBodyType,
  delay,
  HttpResponse,
  type HttpResponseResolver,
  type PathParams
} from 'msw';

export const handleSavefileExport: HttpResponseResolver = async () => {
  await delay(2000);

  return HttpResponse.json({ someData: 'true' });
};

export const handleSavefileEdit: HttpResponseResolver<
  PathParams,
  DefaultBodyType,
  undefined
> = async () => {
  await delay(2000);

  return new HttpResponse(null, { status: 200 });
};
