<script lang="ts" generics="T">
  import type { Field } from 'svelte-form-helper';

  import type { SelectItem } from './types.ts';

  // svelte eslint plugin doesn't appear to be compatible with generic syntax
  /* eslint-disable no-undef */
  export let items: SelectItem<T>[];
  export let value: T | '' = '';
  /* eslint-enable no-undef */

  export let id: string;
  export let placeholder: string | undefined = undefined;

  export let field: Field;

  let className: string | undefined = undefined;
  export { className as class };

  $: {
    if (value && !items.some((x) => x.value === value)) {
      value = '';
    }
  }
</script>

<select class={className} {id} bind:value use:field on:change {...$$restProps}>
  <option value="" hidden disabled selected>{placeholder}</option>
  {#each items as item}
    <option value={item.value}>{item.label}</option>
  {/each}
</select>
