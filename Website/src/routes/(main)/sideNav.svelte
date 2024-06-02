<script lang="ts">
  import Routes from '$lib/routes/routes.svelte';
</script>

<div id="navigation" class="hidden flex-col gap-4 py-2 md:flex">
  <nav class="grid gap-1 px-2">
    <Routes />
  </nav>
</div>

<style>
  #navigation {
    border-right: 1px solid;
    border-color: var(--divider);
    position: fixed;
    height: 100%;
    top: var(--header-height);
    width: var(--navigation-width);
    padding: 1rem 0 0 1rem;
  }
</style>
