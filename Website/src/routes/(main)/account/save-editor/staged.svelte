<script lang="ts">
  import NotebookPen from 'lucide-svelte/icons/notebook-pen';
  import { toast } from 'svelte-sonner';

  import type { SaveChangesRequest } from '$main/account/save-editor/present/presentTypes';
  import { Button } from '$shadcn/components/ui/button';
  import * as Card from '$shadcn/components/ui/card';

  import StagedPresent from './present/stagedPresent.svelte';
  import { changesCount, presents } from './stores';

  let anyModifications = $derived($changesCount > 0);

  let loading = $state(false);

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
      body: JSON.stringify(requestBody),
      headers: {
        'Content-Type': 'application/json'
      }
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

<Card.Root class="h-full w-full overflow-hidden">
  <Card.Header>
    <Card.Title id="staged-changes-title">
      <div class="flex flex-row items-center justify-items-start gap-2">
        <NotebookPen aria-hidden={true} size={25} />
        Staged changes
        {#if $changesCount > 90}
          <div class="flex-grow"></div>
          <p class="text-muted-foreground text-sm font-normal">{$changesCount} / 100</p>
        {/if}
      </div>
    </Card.Title>
  </Card.Header>
  <Card.Content class={anyModifications ? 'h-full' : 'pb-0'}>
    <div class="flex gap-3">
      <Button disabled={!anyModifications} variant="outline" onclick={onClickReset}>Reset</Button>
      <Button disabled={!anyModifications || $changesCount > 100} {loading} onclick={onClickSave}>
        Save changes
      </Button>
    </div>
    <br />
    <ul class="flex h-[75%] flex-col gap-2 overflow-y-auto" aria-labelledby="staged-changes-title">
      {#if anyModifications}
        {#each $presents as present (present.id)}
          <StagedPresent {present} />
        {/each}
      {/if}
    </ul>
  </Card.Content>
</Card.Root>
