<script lang="ts" generics="T">
  import type { HTMLSelectAttributes } from 'svelte/elements';
  import type { Field } from 'svelte-form-helper';

  import { cn } from '$shadcn/utils.ts';

  import type { SelectItem } from './types.ts';

  type SelectProps<T> = {
    items: SelectItem<T>[];
    value: T | '';
    id: string;
    placeholder?: string;
    field: Field;
    class?: string;
  } & HTMLSelectAttributes;

  let {
    items,
    value = $bindable(),
    id,
    placeholder,
    field,
    class: className,
    ...restProps
  }: SelectProps<T> = $props();

  $effect(() => {
    if (value && !items.some((x) => x.value === value)) {
      value = '';
    }
  });
</script>

<select
  class={cn(className, 'bg-background dark:bg-input/30')}
  {id}
  bind:value
  use:field
  {...restProps}>
  <option value="" hidden disabled selected>{placeholder}</option>
  {#each items as item (item.value)}
    <option value={item.value}>{item.label}</option>
  {/each}
</select>
