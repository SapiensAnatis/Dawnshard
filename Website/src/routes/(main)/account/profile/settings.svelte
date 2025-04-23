<script lang="ts">
  import Settings from 'lucide-svelte/icons/settings';

  import type { UserProfile } from '$main/account/profile/userProfile.ts';
  import { Button } from '$shadcn/components/ui/button';
  import * as Card from '$shadcn/components/ui/card';
  import { Label } from '$shadcn/components/ui/label';
  import { Switch } from '$shadcn/components/ui/switch';

  let { settings: remoteSettings }: { settings: UserProfile['settings'] } = $props();

  let localSettings = $state(remoteSettings);

  let isChanged = $derived.by(() => {
    for (const setting in remoteSettings) {
      const castedSetting = setting as keyof typeof remoteSettings;

      if (localSettings[castedSetting] !== remoteSettings[castedSetting]) {
        return true;
      }
    }

    return false;
  });

  const handleReset = () => {
    localSettings = remoteSettings;
  };
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
    <div class="mb-4">Use the following settings to customise your gameplay experience.</div>
    <div class="flex items-center gap-2">
      <!-- Would be nice to use a snippet here, but you can't bind to snippet props :( -->
      <div class="flex flex-row-reverse items-center gap-2">
        <Label for="daily-gifts">Receive daily material gifts</Label>
        <Switch id="daily-gifts" bind:checked={localSettings.dailyGifts} />
      </div>
    </div>
  </Card.Content>
  <Card.Footer>
    <div>
      <Button variant="outline" disabled={!isChanged} onclick={handleReset}>Reset</Button>
      <Button disabled={!isChanged}>Save</Button>
    </div>
  </Card.Footer>
</Card.Root>
