<svelte:options immutable={true} />

<script lang="ts">
  import Page from '$lib/components/page.svelte';
  import Typography from '$lib/components/typography.svelte';

  import type { PageData } from './$types';
  import PresentWidget from './presentWidget.svelte';

  export let data: PageData;
</script>

<Page title="Save Editor">
  <div id="about" class="max-w-7xl">
    <Typography typography="h2">About</Typography>
    <p>
      On this page, you can make basic modifications to your account without having to go through
      the save import and export process.
    </p>
    <p>
      The functionality is basic for now, but may be expanded in the future. For more extensive
      modifications, using the Java-based <a
        class="link"
        href="https://github.com/sockperson/DragaliaSaveEditor/releases/latest">
        DragaliaSaveEditor
      </a> by sockperson is recommended, but it requires access to a desktop computer.
    </p>
  </div>

  <br class="my-2" />

  <div id="editor" class="flex">
    <PresentWidget widgetData={data.presentWidgetData} formData={data.form} />
  </div>
</Page>

<style>
  p + p {
    margin-top: 0.5rem;
  }
</style>
