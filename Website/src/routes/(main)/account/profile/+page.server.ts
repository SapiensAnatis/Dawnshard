import type { Actions } from './$types';

export const actions = {
  settings: async ({ request, fetch, url }) => {
    const data = await request.formData();
    const dataObject = Object.fromEntries(data);
    let requestObject = {};

    // Convert values to actual booleans
    for (const [key, value] of Object.entries(dataObject)) {
      requestObject = { ...requestObject, [key]: value === 'on' };
    }

    const fetchRequest = new Request(new URL('/api/settings', url.origin), {
      method: 'PUT',
      body: JSON.stringify(requestObject),
      headers: {
        'Content-Type': 'application/json'
      }
    });

    const response = await fetch(fetchRequest);

    if (!response.ok) {
      console.log(await response.text());
      throw new Error(`Failed to save settings: status ${response.status}`);
    }

    return { ...requestObject };
  }
} satisfies Actions;
