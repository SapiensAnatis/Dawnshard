const baasApiCall = async <TResponse>(
  relativeUrl: string,
  request: RequestInit,
) => {
  const url = new URL(relativeUrl, import.meta.env.VITE_BAAS_URL);
  const proxyUrl = new URL(`https://cors-anywhere.herokuapp.com/${url}`);

  const response = await fetch(new Request(proxyUrl, request));

  if (!response.ok) {
    console.error(
      `Request to ${url} failed: ${response.status} (${response.statusText})`,
    );
    throw Error(
      `Request to ${url} failed: ${response.status} (${response.statusText})`,
    );
  }

  const decoded = await response.json();
  return decoded as TResponse;
};

export default baasApiCall;
