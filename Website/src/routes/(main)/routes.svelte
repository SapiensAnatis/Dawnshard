<script lang="ts">
  import { Close } from '$shadcn/components/ui/drawer';

  import RouteButton from './routeButton.svelte';
  import { routeGroups } from './routes.ts';

  let { hasValidJwt, drawer }: { hasValidJwt: boolean; drawer?: boolean } = $props();
</script>

{#each routeGroups as routeGroup (routeGroup.title)}
  {#if !routeGroup.requireAuth || (routeGroup.requireAuth && hasValidJwt)}
    <div>
      <h2 class="mb-1 scroll-m-20 text-lg font-semibold tracking-tight">{routeGroup.title}</h2>
      {#if drawer}
        <Close class="flex w-full flex-col">
          {#each routeGroup.routes as route (route.href)}
            <RouteButton {route} />
          {/each}
        </Close>
      {:else}
        {#each routeGroup.routes as route (route.href)}
          <RouteButton {route} />
        {/each}
      {/if}
    </div>
  {/if}
{/each}

<style>
  div + div {
    margin-top: 1rem;
  }
</style>
