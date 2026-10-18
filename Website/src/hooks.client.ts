import { HttpNetworkFrame } from 'msw/experimental';

import { PUBLIC_ENABLE_MSW } from '$env/static/public';

if (PUBLIC_ENABLE_MSW === 'true') {
  const { worker } = await import('./mocks/browser');

  await worker.start({
    onUnhandledFrame({ frame, defaults }) {
      if (frame instanceof HttpNetworkFrame && frame.data.request.url.includes('api')) {
        defaults.warn();
      }
    }
  });
}
