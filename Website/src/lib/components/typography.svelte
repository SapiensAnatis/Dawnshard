<script lang="ts">
  import { cn } from '$shadcn/utils.ts';

  export let typography: 'h1' | 'h2' | 'h3' | 'h4';
  export let id: string | undefined = undefined;

  let className = '';
  export { className as class };
</script>

{#if typography === 'h1'}
  <h1 {id}><slot /></h1>
{:else if typography === 'h2'}
  <h2 {id} class={cn(className, 'mb-2 text-2xl font-bold')}><slot /></h2>
{:else if typography === 'h3'}
  <h3 {id} class={cn(className, 'mb-2 text-2xl font-semibold')}><slot /></h3>
{:else if typography === 'h4'}
  <h4 {id} class={cn(className, 'mb-2 text-xl font-semibold')}><slot /></h4>
{/if}
