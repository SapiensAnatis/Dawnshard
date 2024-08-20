<script lang="ts" generics="T">
  import type { Action } from 'svelte/action';

  import { cn } from '$shadcn/utils.js.ts';

  import type { SelectItem } from './types.ts';

  // svelte eslint plugin doesn't appear to be compatible with generic syntax
  /* eslint-disable no-undef */
  export let items: SelectItem<T>[];
  export let value: T | '' = '';
  /* eslint-enable no-undef */

  export let id: string;
  export let placeholder: string | undefined = undefined;

  export let action: Action;

  let className: string | undefined = undefined;
  export { className as class };

  $: {
    if (value && !items.some((x) => x.value === value)) {
      value = '';
    }
  }
</script>

<select
  class={cn(
    className,
    'flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50'
  )}
  class:placeholder-selected={value === ''}
  {id}
  bind:value
  use:action
  {...$$restProps}>
  <option value="" hidden disabled selected>{placeholder}</option>
  {#each items as item}
    <option value={item.value}>{item.label}</option>
  {/each}
</select>

<style>
  select {
    background-color: hsl(var(--background));
    padding: 0.5rem;
    border-radius: 0.4rem;
    border: 1px hsl(var(--border)) solid;
    display: grid;
    grid-template-areas: 'select';
    align-items: center;
  }

  .placeholder-selected {
    color: hsl(var(--muted-foreground));
  }
</style>
