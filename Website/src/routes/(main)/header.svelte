<script lang="ts">
  import { toggleMode } from 'mode-watcher';
  import { Button } from '$shadcn/components/ui/button';
  import * as Drawer from '$shadcn/components/ui/drawer';
  import { Sun, Moon, Menu, Close } from '$lib/icons';
  import Routes from '$lib/components/routes.svelte';
  import { onMount } from 'svelte';
  import LoginButton from './loginButton.svelte';

	let enhance = false;

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
      <h1 class="scroll-m-20 text-2xl font-bold tracking-tight md:text-3xl">Dawnshard</h1>
      <div class="flex-grow" />
      <Button on:click={toggleMode} variant="outline" size="icon">
        <Sun
          class="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0"
          aria-hidden
        />
        <Moon
          class="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100"
          aria-hidden
        />
        <span class="sr-only">Toggle theme</span>
      </Button>
      <LoginButton />

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
    <h1 class="scroll-m-20 text-2xl font-bold tracking-tight md:text-3xl">Dawnshard</h1>
    <div style:flex-grow="1" />
    <LoginButton />
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
