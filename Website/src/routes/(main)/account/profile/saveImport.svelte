<script lang="ts">
  import Download from 'lucide-svelte/icons/download';

  import { PUBLIC_BAAS_URL } from '$env/static/public';
  import { Button } from '$shadcn/components/ui/button';
  import * as Card from '$shadcn/components/ui/card';

  import renderDate from './renderDate.ts';

  export let lastSaveImportTime: Date | null;
</script>

<Card.Root>
  <Card.Header>
    <Card.Title level={2}>
      <div class="flex flex-row items-center justify-items-start gap-2">
        <Download aria-hidden={true} size={25} />
        Save import
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
