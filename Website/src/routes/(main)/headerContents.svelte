<script lang="ts">
  import { toggleMode } from 'mode-watcher';
  import Moon from 'lucide-svelte/icons/moon';
  import Sun from 'lucide-svelte/icons/sun';
  import { Button } from '$shadcn/components/ui/button';
  import { page } from '$app/stores';

  export let hasValidJwt: boolean;
</script>

<h1 class="scroll-m-20 text-2xl font-bold tracking-tight md:text-3xl">Dawnshard</h1>
<div class="flex-grow" />
<Button on:click={toggleMode} variant="outline" size="icon">
  <Sun
    class="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0"
    aria-hidden />
  <Moon
    class="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100"
    aria-hidden />
  <span class="sr-only">Toggle theme</span>
</Button>

{#if hasValidJwt}
  <Button href="/logout" variant="secondary" data-sveltekit-reload>Log out</Button>
{:else}
  <Button href={`/login?originalPage=${$page.url.pathname}`}>Login</Button>
{/if}
