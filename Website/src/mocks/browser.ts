import { setupWorker } from 'msw/browser';

import { handlers } from './handlers/handlers';

export const worker = setupWorker(...handlers);
