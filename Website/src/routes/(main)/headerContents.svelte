<script lang="ts">
  import { toggleMode } from 'mode-watcher';
  import { Moon, Sun } from '$lib/icons.js';
  import { Button } from '$shadcn/components/ui/button';
  import { invalidate } from '$app/navigation';
  import Cookies from '$lib/cookies';
  import { page } from '$app/stores';

  export let hasValidJwt: boolean;
</script>

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

{#if hasValidJwt}
  <Button
    href="/logout"
    variant="secondary"
    on:click={() => invalidate(`cookie:${Cookies.IdToken}`)}>Log out</Button
  >
{:else}
  <Button href={`/login?originalPage=${$page.url.pathname}`}>Login</Button>
{/if}
