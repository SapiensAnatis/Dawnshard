<script lang="ts">
  import NotebookPen from 'lucide-svelte/icons/notebook-pen';
  import { toast } from 'svelte-sonner';

  import type { SaveChangesRequest } from '$main/account/save-editor/presentTypes.ts';
  import { Button } from '$shadcn/components/ui/button';
  import * as Card from '$shadcn/components/ui/card';

  import StagedPresent from './stagedPresent.svelte';
  import { presents } from './stores';

  $: anyModifications = $presents.length > 0;

  let saveChangesPromise: Promise<void> | null = null;

  const onClickReset = () => {
    presents.set([]);
  };

  const onClickSave = () => {
    if (!anyModifications) return;
    const requestBody = {
      presents: $presents
    };

    saveChangesPromise = saveChanges(requestBody);
  };

  const saveChanges = async (requestBody: SaveChangesRequest) => {
    const request = new Request('/api/savefile/edit', {
      method: 'POST',
      body: JSON.stringify(requestBody)
    });

    const response = await fetch(request);

    if (response.ok) {
      toast.success('Successfully edited save!');
      onClickReset();
    } else {
      throw new Error(`Savefile edit request failed with status ${response.status}`);
    }
  };
</script>

<Card.Root class="w-full">
  <Card.Header>
    <Card.Title>
      <div class="flex flex-row items-center justify-items-start gap-2">
        <NotebookPen aria-hidden={true} size={25} />
        <h2 class="m-0 text-xl font-bold">Staged changes</h2>
      </div>
    </Card.Title>
  </Card.Header>
  <Card.Content class="flex flex-col gap-2">
    {#if anyModifications}
      {#each $presents as present}
        <StagedPresent {present} />
      {/each}
    {:else}
      <p>Any changes you make will show up here.</p>
    {/if}
  </Card.Content>
  <Card.Footer class="gap-2">
    <Button disabled={!anyModifications} variant="outline" on:click={onClickReset}>Reset</Button>
    {#await saveChangesPromise}
      <Button loading={true}>Save changes</Button>
    {:then _}
      <Button disabled={!anyModifications} on:click={onClickSave}>Save changes</Button>
    {/await}
  </Card.Footer>
</Card.Root>
