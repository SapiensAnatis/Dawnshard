if (import.meta.env.mode === "test") {
  const { worker } = await import('./mocks/browser');

  await worker.start({
    onUnhandledRequest(request, print) {
      // Do not warn on unhandled internal Svelte requests.
      // Those are not meant to be mocked.
      if (request.url.includes('svelte')) {
        return;
      }

      print.warning();
    }
  });
}