<script lang="ts">
  import TriangleAlert from 'lucide-svelte/icons/triangle-alert';
  import Upload from 'lucide-svelte/icons/upload';
  import { onMount } from 'svelte';

  import { page } from '$app/stores';
  import { Button } from '$shadcn/components/ui/button';
  import * as Card from '$shadcn/components/ui/card';

  let enhance = false;
  let loading = false;
  let savefileExportPromise: Promise<void> | null = null;

  const savefileExportUrl = new URL('/api/savefile/export', $page.url.origin);

  const getSavefile = async () => {
    loading = true;

    const response = await fetch(savefileExportUrl);

    loading = false;

    if (!response.ok) {
      throw new Error(`Savefile export failed with status ${response.status}`);
    }

    const fileContents = await response.blob();
    const a = document.createElement('a');
    const url = URL.createObjectURL(fileContents);
    a.href = url;
    a.download = 'savedata.txt';
    a.click();
    URL.revokeObjectURL(url);
  };

  onMount(() => {
    enhance = true;
  });
</script>

<Card.Root class="flex flex-col">
  <Card.Header>
    <Card.Title>
      <div class="flex flex-row items-center justify-items-start gap-2">
        <Upload aria-hidden={true} size={25} />
        <h2>Save export</h2>
      </div>
    </Card.Title>
  </Card.Header>
  <Card.Content>
    <div>
      <p class="mb-2">
        You can use the below button to export your save. This allows you to transfer it to another
        server, or to edit it and re-import it.
      </p>
      <p>
        For a tool to ease the process of editing your save, check out the
        <a class="link" href="https://sockperson.github.io/Vite-DragaliaSaveEditor/">
          DragaliaSaveEditor
        </a> by sockperson.
      </p>
    </div>
  </Card.Content>
  <!-- Spacer to align 'visit BaaS' and 'export save' buttons when horizontal -->
  <div class="grow"></div>
  <Card.Footer>
    {#if enhance}
      <div class="flex items-center gap-2">
        <Button
          variant="secondary"
          onclick={() => (savefileExportPromise = getSavefile())}
          {loading}>
          Export save
        </Button>
        {#await savefileExportPromise catch _}
          <span class="error flex items-center gap-1">
            <TriangleAlert /> Failed to export save!
          </span>
        {/await}
      </div>
    {:else}
      <Button variant="secondary" href={savefileExportUrl.href} download={savefileExportUrl}
        >Export save
      </Button>
    {/if}
  </Card.Footer>
</Card.Root>

<style>
  .error {
    color: crimson;
  }
</style>
