<svelte:options immutable={true} />

<script lang="ts">
  import Page from '$lib/components/page.svelte';
  import Typography from '$lib/components/typography.svelte';
  import { Separator } from '$shadcn/components/ui/separator';

  import type { PageData } from './$types';
  import PresentWidget from './present/presentWidget.svelte';
  import Staged from './staged.svelte';

  export let data: PageData;
</script>

<Page title="Save Editor">
  <div id="about" class="max-w-7xl">
    <Typography typography="h2">About</Typography>
    <p class="mb-2">
      On this page, you can make modifications to your account without having to go through the save
      export and import process.
    </p>
    <p>
      The functionality is basic for now, but may be expanded in the future. For more extensive
      modifications, using the web-based <a
        class="link"
        href="https://sockperson.github.io/Vite-DragaliaSaveEditor/">
        DragaliaSaveEditor
      </a>
      by sockperson is recommended, but it requires importing and exporting your save file. You can
      do this on the <a class="link" href="/account/profile">Profile</a> page.
    </p>
  </div>
  <br />
  <div class="flex flex-col gap-2 md:h-[67.5vh] md:flex-row">
    <div class="w-full">
      <PresentWidget widgetData={data.presentWidgetData} />
    </div>
    <Separator class="mx-3 hidden md:block" orientation="vertical" />
    <Separator class="visible my-2 px-4 md:hidden" orientation="horizontal" />
    <div class="h-full w-full overflow-hidden md:w-[35rem]">
      <Staged />
    </div>
  </div>
</Page>
