<script lang="ts">
  import '../app.css';

  import { onMount } from 'svelte';

  import { PUBLIC_VERSION } from '$env/static/public';

  onMount(() => {
    document.body.classList.add('started');
  });
</script>

<svelte:head>
  <meta property="version" content={PUBLIC_VERSION} />
</svelte:head>

<slot />
