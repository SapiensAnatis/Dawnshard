<script lang="ts">
  import Download from 'lucide-svelte/icons/download';

  import { PUBLIC_BAAS_URL } from '$env/static/public';
  import { Button } from '$shadcn/components/ui/button';
  import * as Card from '$shadcn/components/ui/card';

  export let lastSaveImportTime: Date;

  const renderDate = (date: Date) => {
    if (date.valueOf() < new Date(1970, 1, 1).valueOf()) {
      return 'Never!';
    }

    return date.toLocaleString(undefined, {
      dateStyle: 'medium',
      timeStyle: 'short'
    });
  };
</script>

<Card.Root>
  <Card.Header>
    <Card.Title>
      <div class="flex flex-row items-center justify-items-start gap-2">
        <Download size={25} />
        <h2 class="m-0 text-xl font-bold">Save import</h2>
      </div>
    </Card.Title>
  </Card.Header>
  <Card.Content>
    <div>
      <p>You can import your save by visiting the BaaS and uploading a file.</p>
      <p class="mt-2 font-semibold">Last save import time</p>
      <p>{renderDate(lastSaveImportTime)}</p>
    </div>
  </Card.Content>
  <Card.Footer>
    <Button variant="secondary" href={PUBLIC_BAAS_URL}>Visit the BaaS</Button>
  </Card.Footer>
</Card.Root>
