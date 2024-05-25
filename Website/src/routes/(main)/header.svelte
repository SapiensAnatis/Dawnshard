<script lang="ts">
  import { Button } from '$shadcn/components/ui/button';
  import * as Drawer from '$shadcn/components/ui/drawer';
  import { Menu, Close } from '$lib/icons';
  import Routes from '$lib/components/routes.svelte';
  import { onMount } from 'svelte';
  import HeaderContents from './headerContents.svelte';

  let enhance = false;

  export let hasValidJwt: boolean;

  onMount(() => {
    enhance = true;
  });
</script>

{#if enhance}
  <Drawer.Root direction="left">
    <header id="header" class="gap-1 bg-background px-1 md:gap-2 md:px-3">
      <Drawer.Trigger class="md:hidden">
        <Button variant="ghost" class="md:hidden">
          <Menu />
        </Button>
      </Drawer.Trigger>
      <HeaderContents {hasValidJwt} />

      <Drawer.Portal class="md:hidden">
        <Drawer.Content
          id="drawer-content"
          class="fixed bottom-0 left-0 top-0 mt-0 w-[75%] bg-background pl-6 pr-2 pt-2"
        >
          <div id="my-content" class="flex flex-col">
            <Drawer.Close class="self-end">
              <Button variant="ghost">
                Close <Close class="ml-2 mt-0.5 h-5 w-5" />
              </Button>
            </Drawer.Close>
            <Routes />
          </div>
        </Drawer.Content>
      </Drawer.Portal>
    </header>
  </Drawer.Root>
{:else}
  <header id="header" class="gap-1 bg-background px-1 md:gap-2 md:px-3">
    <Button variant="ghost" class="md:hidden" href="/navigation">
      <Menu />
    </Button>
    <HeaderContents {hasValidJwt} />
  </header>
{/if}

<style>
  #header {
    display: flex;
    flex-grow: 1;
    justify-content: space-between;
    align-items: center;
    border-bottom: 1px solid;
    border-color: var(--divider);
    position: fixed;
    width: 100vw;
    height: var(--header-height);
  }

  /* Prevent draggable bar from rendering */
  :global(#drawer-content > :not(#my-content)) {
    display: none;
  }
</style>
