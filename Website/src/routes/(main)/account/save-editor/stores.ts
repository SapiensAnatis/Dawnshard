import { derived, writable } from 'svelte/store';

import type { PresentFormSubmission } from '$main/account/save-editor/present/presentTypes.ts';

export const presents = writable<PresentFormSubmission[]>([]);

export const changesCount = derived(presents, (presents) => presents.length);
