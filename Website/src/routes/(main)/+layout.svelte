<script lang="ts">
  import { ModeWatcher } from 'mode-watcher';

  import Header from './header.svelte';
  import SideNav from './sideNav.svelte';
  import type { LayoutData } from './$types';

  export let data: LayoutData;
</script>

<ModeWatcher />
<Header hasValidJwt={data.hasValidJwt} />
<SideNav />

<main class="pl-0 md:pl-[var(--navigation-width)]" style:padding-top="var(--header-height)">
  <slot />
</main>
