import { decodeAsync } from "@msgpack/msgpack";

const apiCall = async <TResponse>(path: string) => {
  console.debug(import.meta.env.API_URL, path);
  const url = new URL(path, import.meta.env.VITE_API_URL);
  console.debug(url);

  const response = await fetch(url);
  console.log(response);

  if (!response.ok) {
    console.error(
      `Request to ${url} failed: ${response.status} (${response.statusText})`,
    );
    throw Error("API call failed");
  }

  if (!response.body) {
    console.error(`Request to ${url} failed: empty response body`);
    throw Error("API cal l failed");
  }

  return (await decodeAsync(response.body)) as TResponse;
};

export default apiCall;
