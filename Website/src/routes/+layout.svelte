<script lang="ts">
  import '../app.pcss';

  // @ts-expect-error __APP_VERSION__ is replaced by svelte-preprocess
  // eslint-disable-next-line no-undef
  const version = __APP_VERSION__;
</script>

<svelte:head>
  <meta property="version" content={version} />
</svelte:head>

<slot />
