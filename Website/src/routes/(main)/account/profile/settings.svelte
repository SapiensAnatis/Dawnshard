<script lang="ts">
  import Settings from 'lucide-svelte/icons/settings';

  import * as Card from '$shadcn/components/ui/card';
  import { Label } from '$shadcn/components/ui/label';
  import { Switch } from '$shadcn/components/ui/switch';

  let checked = $state(false);
</script>

<Card.Root>
  <Card.Header>
    <Card.Title level={2}>
      <div class="flex flex-row items-center justify-items-start gap-2">
        <Settings aria-hidden={true} size={25} />
        Settings
      </div>
    </Card.Title>
  </Card.Header>
  <Card.Content>
    <div class="mb-4">Use the following settings to alter your gameplay experience.</div>
    <div class="flex items-center gap-2">
      <Switch id="daily-gifts" bind:checked></Switch>
      <Label for="daily-gifts">Receive daily material gifts</Label>
    </div>
  </Card.Content>
</Card.Root>
