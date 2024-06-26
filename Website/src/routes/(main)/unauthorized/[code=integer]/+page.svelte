<script lang="ts">
  import { page } from '$app/stores';
</script>

<div class="center">
  <h1 class="mb-2 text-3xl">You are not authorized to access this page.</h1>
  <p class="text-lg">Error code: {$page.params.code}</p>
  {#if $page.params.code === '404'}
    <p class="mt-4">
      This indicates that your BaaS account does not have an associated Dawnshard save file. Try
      again after linking this account to an installation of the mobile game.
    </p>
  {/if}
</div>

<style>
  .center {
    text-align: center;
    margin-top: 5%;
  }
</style>
