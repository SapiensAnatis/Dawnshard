<script lang="ts">
  import NotebookPen from 'lucide-svelte/icons/notebook-pen';
  import { toast } from 'svelte-sonner';

  import type { SaveChangesRequest } from '$main/account/save-editor/presentTypes.ts';
  import { Button } from '$shadcn/components/ui/button';
  import * as Card from '$shadcn/components/ui/card';

  import StagedPresent from './stagedPresent.svelte';
  import { presents } from './stores';

  $: anyModifications = $presents.length > 0;

  let loading = false;

  const onClickReset = () => {
    presents.set([]);
  };

  const onClickSave = () => {
    if (!anyModifications) return;
    const requestBody = {
      presents: $presents
    };

    saveChanges(requestBody);
  };

  const saveChanges = async (requestBody: SaveChangesRequest) => {
    loading = true;

    const request = new Request('/api/savefile/edit', {
      method: 'POST',
      body: JSON.stringify(requestBody)
    });

    const response = await fetch(request);

    loading = false;

    if (response.ok) {
      toast.success('Successfully edited save');
      onClickReset();
    } else {
      toast.error('Failed to edit save');
      // eslint-disable-next-line no-console
      console.error('Savefile edit request failed with status', response.status);
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
  <Card.Content class={anyModifications ? '' : 'pb-0'}>
    <div class="flex gap-3">
      <Button disabled={!anyModifications} variant="outline" on:click={onClickReset}>Reset</Button>
      <Button disabled={!anyModifications} {loading} on:click={onClickSave}>Save changes</Button>
    </div>
    <br />
    <div class="flex max-h-[35vh] flex-col gap-2 overflow-y-auto">
      {#if anyModifications}
        {#each $presents as present}
          <StagedPresent {present} />
        {/each}
      {/if}
    </div>
  </Card.Content>
</Card.Root>
