<script lang="ts">
  import { createEventDispatcher } from 'svelte';

  import { Button } from '$shadcn/components/ui/button';
  import { routeGroups } from './routes.ts';

  export let hasValidJwt: boolean;

  const dispatch = createEventDispatcher();

  const onClick = () => {
    dispatch('navigate');
  };
</script>

{#each routeGroups as routeGroup}
  {#if !routeGroup.requireAuth || (routeGroup.requireAuth && hasValidJwt)}
    <div>
      <h2 class="mb-1 scroll-m-20 text-lg font-semibold tracking-tight">{routeGroup.title}</h2>
      {#each routeGroup.routes as route}
        <Button
          href={route.href}
          variant="ghost"
          class="h-11 w-[90%] justify-start"
          on:click={onClick}>
          <svelte:component this={route.icon} class="mr-2 size-6" aria-hidden="true" />
          {route.title}
        </Button>
      {/each}
    </div>
  {/if}
{/each}

<style>
  div + div {
    margin-top: 1rem;
  }
</style>
