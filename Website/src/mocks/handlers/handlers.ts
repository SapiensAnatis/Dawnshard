import { http } from 'msw';

import handleNews from './news.ts';

export const handlers = [
  http.get('/api/news', handleNews),
  http.get('http://localhost:5000/api/news', handleNews)
];
