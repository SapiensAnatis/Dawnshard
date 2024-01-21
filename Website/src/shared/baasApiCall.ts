const baasApiCall = async <TResponse>(
  relativeUrl: string,
  request: RequestInit,
) => {
  const url = new URL(relativeUrl, import.meta.env.VITE_BAAS_URL);

  const response = await fetch(new Request(url, request));

  if (!response.ok) {
    console.error(
      `Request to ${url} failed: ${response.status} (${response.statusText})`,
    );
    throw Error("BaaS API call failed");
  }

  const decoded = await response.json();
  return decoded as TResponse;
};

export default baasApiCall;
