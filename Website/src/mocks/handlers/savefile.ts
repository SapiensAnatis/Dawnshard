import { delay, HttpResponse, type HttpResponseResolver } from 'msw';

const handleSavefileExport: HttpResponseResolver = async () => {
  await delay(2000);

  return HttpResponse.json({ someData: 'true' });
};

export default handleSavefileExport;
