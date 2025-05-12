<script lang="ts">
  import Download from 'lucide-svelte/icons/download';

  import { PUBLIC_BAAS_URL } from '$env/static/public';
  import { Button } from '$shadcn/components/ui/button';
  import * as Card from '$shadcn/components/ui/card';

  import renderDate from './renderDate.ts';

  let { lastSaveImportTime }: { lastSaveImportTime: Date | null } = $props();

  const formattedLastSaveImportTime = $derived(renderDate(lastSaveImportTime));
</script>

<Card.Root class="flex flex-col">
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
      <p>You can import a save file by visiting the BaaS and uploading a file.</p>
      <p class="mt-2 font-semibold">Last save import time</p>
      <p>{formattedLastSaveImportTime}</p>
    </div>
  </Card.Content>
  <!-- Spacer to align 'visit BaaS' and 'export save' buttons when horizontal -->
  <div class="grow"></div>
  <Card.Footer>
    <Button href={PUBLIC_BAAS_URL}>Visit the BaaS</Button>
  </Card.Footer>
</Card.Root>
