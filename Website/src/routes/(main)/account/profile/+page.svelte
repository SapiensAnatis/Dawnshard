<script lang="ts">
  import Page from '$lib/components/page.svelte';

  import type { PageData } from './$types';
  import SaveExport from './saveExport.svelte';
  import SaveImport from './saveImport.svelte';
  import UserInformation from './userInformation.svelte';

  export let data: PageData;
</script>

<Page title="Profile">
  <div class="w-full px-3 lg:w-[75%]">
    <div class="card">
      <UserInformation user={data.user} userProfile={data.userProfile} />
    </div>
    <div class="card">
      <SaveExport />
    </div>
    <div class="card">
      <SaveImport lastSaveImportTime={data.userProfile.lastSaveImportTime} />
    </div>
  </div>
</Page>

<style>
  .card + .card {
    margin-top: 1rem;
  }
</style>
