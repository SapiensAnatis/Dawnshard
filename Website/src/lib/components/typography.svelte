<script lang="ts">
  import { cn } from '$shadcn/utils.js';

  export let typography: 'h1' | 'h2' | 'h3' | 'h4';

  let className = '';
  export { className as class };
</script>

{#if typography === 'h1'}
  <h1><slot /></h1>
{:else if typography === 'h2'}
  <h2 class={cn(className, 'mb-2 text-3xl font-bold')}><slot /></h2>
{:else if typography === 'h3'}
  <h3 class={cn(className, 'mb-2 text-2xl font-semibold')}><slot /></h3>
{:else if typography === 'h4'}
  <h4 class={cn(className, 'mb-2 text-xl font-semibold')}><slot /></h4>
{/if}
