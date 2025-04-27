<script lang="ts">
  import '../app.pcss';

  import { PUBLIC_VERSION } from '$env/static/public';

  import { onMount } from 'svelte';

  onMount(() => {
    document.body.classList.add('started');
  });
</script>

<svelte:head>
  <meta property="version" content={PUBLIC_VERSION} />
</svelte:head>

<slot />
