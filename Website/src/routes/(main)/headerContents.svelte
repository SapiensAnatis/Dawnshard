<script lang="ts">
  import Moon from '@lucide/svelte/icons/moon';
  import Sun from '@lucide/svelte/icons/sun';
  import { toggleMode } from 'mode-watcher';
  import { onMount } from 'svelte';

  import LanguageSelector from '$main/languageSelector.svelte';
  import LogInOrOutButton from '$main/logInOrOutButton.svelte';
  import { Button } from '$shadcn/components/ui/button';

  let initialized = $state(false);
  let { hasValidJwt }: { hasValidJwt: boolean } = $props();

  onMount(() => {
    // We use this in the tests to delay clicking on the theme toggle until mode-watcher is likely
    // to be ready to work.
    initialized = true;
  });
</script>

<h1 class="scroll-m-20 text-2xl font-bold tracking-tight md:text-3xl">Dawnshard</h1>
<div class="grow"></div>
<div class="hidden md:block"><LanguageSelector /></div>
<Button onclick={toggleMode} variant="outline" size="icon" data-loaded={initialized}>
  <Sun
    class="absolute size-5 scale-100 rotate-0 transition-all dark:scale-0 dark:-rotate-90"
    size="400"
    aria-hidden />
  <Moon
    class="absolute size-5 scale-0 rotate-90 transition-all dark:scale-100 dark:rotate-0"
    aria-hidden />
  <span class="sr-only">Toggle theme</span>
</Button>
<LogInOrOutButton {hasValidJwt} />
