<script>
  import { routeGroups } from '../routes';
  import { Button } from '$shadcn/components/ui/button';
  import { cn } from '$shadcn/utils.js';
</script>

{#each routeGroups as routeGroup}
  <h2 class="text-m scroll-m-20 font-semibold tracking-tight">{routeGroup.title}</h2>
  {#each routeGroup.routes as route}
    <Button href={route.href} variant="ghost" size="sm" class={cn('justify-start')}>
      <svelte:component this={route.icon} class="mr-2 size-4" aria-hidden="true" />
      {route.title}
    </Button>
  {/each}
{/each}
