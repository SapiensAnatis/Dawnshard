import { PUBLIC_ENABLE_MSW } from '$env/static/public';

if (PUBLIC_ENABLE_MSW === 'true') {
  const { worker } = await import('./mocks/browser');

  await worker.start({
    onUnhandledRequest(request, print) {
      if (request.url.includes('api')) {
        print.warning();
      }
    }
  });
}
