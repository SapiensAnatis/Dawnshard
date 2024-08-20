import { writable } from 'svelte/store';

import type { PresentFormSubmission } from '$main/account/save-editor/presentTypes.ts';

export const presents = writable<PresentFormSubmission[]>([]);
