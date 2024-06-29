<script lang="ts">
  import Routes from '$main/routes.svelte';

  import type { PageData } from './$types';

  export let data: PageData;
</script>

<nav class="flex flex-col justify-center gap-1 p-3">
  <h1 class="mb-4 text-3xl font-bold">Navigation</h1>
  <Routes hasValidJwt={data.hasValidJwt} />
</nav>
