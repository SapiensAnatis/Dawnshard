<script lang="ts">
  import Moon from 'lucide-svelte/icons/moon';
  import Sun from 'lucide-svelte/icons/sun';
  import { toggleMode } from 'mode-watcher';

  import { page } from '$app/stores';
  import { Button } from '$shadcn/components/ui/button';
  import { onMount } from 'svelte';

  let jsLoaded = false;

  export let hasValidJwt: boolean;

  onMount(() => {
    // We use this in the tests to delay clicking on the theme toggle until mode-watcher is likely
    // to be ready to work.
    jsLoaded = true;
  });
</script>

<h1 class="scroll-m-20 text-2xl font-bold tracking-tight md:text-3xl">Dawnshard</h1>
<div class="flex-grow" />
<Button on:click={toggleMode} variant="outline" size="icon" data-loaded={jsLoaded}>
  <Sun
    class="absolute size-5 rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0"
    aria-hidden />
  <Moon
    class="absolute size-5 rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100"
    aria-hidden />
  <span class="sr-only">Toggle theme</span>
</Button>

{#if hasValidJwt}
  <Button href="/logout" variant="secondary" data-sveltekit-reload>Log out</Button>
{:else}
  <Button href={`/login?originalPage=${$page.url.pathname}`}>Login</Button>
{/if}
