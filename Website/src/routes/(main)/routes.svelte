<script lang="ts">
  import { Button } from '$shadcn/components/ui/button';
  import { cn } from '$shadcn/utils.js.ts';

  import { routeGroups } from './routes.ts';

  export let hasValidJwt: boolean;
</script>

{#each routeGroups as routeGroup}
  {#if !routeGroup.requireAuth || (routeGroup.requireAuth && hasValidJwt)}
    <h2 class="text-m scroll-m-20 font-semibold tracking-tight">{routeGroup.title}</h2>
    {#each routeGroup.routes as route}
      <Button href={route.href} variant="ghost" size="sm" class={cn('justify-start', 'w-[90%]')}>
        <svelte:component this={route.icon} class="mr-2 size-4" aria-hidden="true" />
        {route.title}
      </Button>
    {/each}
  {/if}
{/each}
