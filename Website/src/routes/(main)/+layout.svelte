<script lang="ts">
  import { ModeWatcher } from 'mode-watcher';

  import type { LayoutData } from './$types';
  import Header from './header.svelte';
  import SideNav from './sideNav.svelte';

  export let data: LayoutData;
</script>

<ModeWatcher />
<Header hasValidJwt={data.hasValidJwt} />
<SideNav hasValidJwt={data.hasValidJwt} />

<main class="pl-0 md:pl-[var(--navigation-width)]" style:padding-top="var(--header-height)">
  <slot />
</main>
