<script lang="ts">
  import '../app.pcss';

  import { PUBLIC_VERSION } from '$env/static/public';
</script>

<svelte:head>
  <meta property="version" content={PUBLIC_VERSION} />
</svelte:head>

<slot />
