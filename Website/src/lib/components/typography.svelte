<script lang="ts">
  import type { HTMLAttributes } from 'svelte/elements';

  import { cn } from '$shadcn/utils.ts';

  let {
    class: className,
    id,
    typography,
    children
  }: {
    typography: 'h1' | 'h2' | 'h3' | 'h4';
  } & Pick<HTMLAttributes<HTMLParagraphElement>, 'children' | 'class' | 'id'> = $props();
</script>

{#if typography === 'h1'}
  <h1 {id}>
    {@render children?.()}
  </h1>
{:else if typography === 'h2'}
  <h2 {id} class={cn(className, 'mb-2 text-3xl font-bold')}>
    {@render children?.()}
  </h2>
{:else if typography === 'h3'}
  <h3 {id} class={cn(className, 'mb-2 text-2xl font-semibold')}>
    {@render children?.()}
  </h3>
{:else if typography === 'h4'}
  <h4 {id} class={cn(className, 'mb-2 text-xl font-semibold')}>
    {@render children?.()}
  </h4>
{/if}
