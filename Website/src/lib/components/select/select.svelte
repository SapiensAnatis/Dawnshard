<script lang="ts" generics="T">
  import type { Field } from 'svelte-form-helper';

  import type { SelectItem } from './types.ts';

  type SelectProps = {
    items: SelectItem<T>[];
    value: T | '';
    id: string;
    placeholder?: string;
    field: Field;
    class?: string;
  };

  let {
    items,
    value,
    id,
    placeholder,
    field,
    class: className,
    ...restProps
  }: SelectProps = $props();

  $effect(() => {
    if (value && !items.some((x) => x.value === value)) {
      value = '';
    }
  });
</script>

<select class={className} {id} bind:value use:field {...restProps}>
  <option value="" hidden disabled selected>{placeholder}</option>
  {#each items as item (item.value)}
    <option value={item.value}>{item.label}</option>
  {/each}
</select>
