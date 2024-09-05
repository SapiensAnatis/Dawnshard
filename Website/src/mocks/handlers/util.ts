import { type DefaultBodyType, type JsonBodyType, type StrictRequest } from 'msw';

const data = import.meta.glob('./data/**/*.json', { import: 'default' });

export const getData = async (
  request: StrictRequest<DefaultBodyType>
): Promise<JsonBodyType | null> => {
  const path = new URL(request.url).pathname.replace('/api/', '');
  const importPath = `./data/${path}.json`;

  try {
    const dataEntry = data[importPath];

    if (!dataEntry) {
      console.warn('Failed to get data from path:', importPath);
      return null;
    }

    // @ts-expect-error vite returns unknown, but we know this to be json
    return await dataEntry();
  } catch (e) {
    console.error('Failed to get data:', e);
  }
};
