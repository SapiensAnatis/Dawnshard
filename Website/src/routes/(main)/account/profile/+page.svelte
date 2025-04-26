<script lang="ts">
  import Page from '$lib/components/page.svelte';

  import type { PageProps } from './$types';
  import SaveExport from './saveExport.svelte';
  import SaveImport from './saveImport.svelte';
  import Settings from './settings.svelte';
  import UserInformation from './userInformation.svelte';

  const { data }: PageProps = $props();
</script>

<Page title="Profile">
  <div class="flex w-full flex-col gap-2 px-3 lg:w-[75%]">
    <div class="section">
      <UserInformation user={data.user} userProfile={data.userProfile} />
    </div>
    <div class="grid grid-cols-1 gap-2 md:grid-cols-2">
      <SaveImport lastSaveImportTime={data.userProfile.lastSaveImportTime} />
      <SaveExport />
    </div>
    <div class="section">
      <Settings settings={data.userProfile.settings} />
    </div>
  </div>
</Page>

<style>
</style>
