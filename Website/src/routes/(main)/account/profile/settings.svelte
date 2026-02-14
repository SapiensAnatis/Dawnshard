<script lang="ts">
  import type { ActionResult } from '@sveltejs/kit';
  import Settings from 'lucide-svelte/icons/settings';
  import { toast } from 'svelte-sonner';

  import { applyAction, enhance } from '$app/forms';
  import type { UserProfile } from '$main/account/profile/userProfile.ts';
  import { Button } from '$shadcn/components/ui/button';
  import * as Card from '$shadcn/components/ui/card';
  import { Label } from '$shadcn/components/ui/label';
  import { Switch } from '$shadcn/components/ui/switch';

  type SettingsType = UserProfile['settings'];

  let { settings }: { settings: SettingsType } = $props();

  let remoteSettings = $derived(settings);
  // svelte-ignore state_referenced_locally
  let localSettings = $state(settings);

  let isChanged = $derived.by(() => {
    for (const setting in remoteSettings) {
      const castedSetting = setting as keyof typeof remoteSettings;

      if (localSettings[castedSetting] !== remoteSettings[castedSetting]) {
        return true;
      }
    }

    return false;
  });

  // The form data is represented as an object like { dailyGifts: 'on' }, in which a key
  // not being present means it is toggled off.
  type FormReturn = Partial<Record<keyof SettingsType, 'on'>>;

  const mapFormResultToState = (
    result: Extract<ActionResult<FormReturn, undefined>, { type: 'success' }>
  ) => {
    if (!result.data) {
      throw new Error('No data returned by successful form action');
    }

    let acc = { ...localSettings };

    for (const setting in localSettings) {
      acc = { ...acc, [setting]: setting in result.data };
    }

    return acc;
  };

  const handleReset = () => {
    localSettings = { ...remoteSettings };
  };
</script>

<Card.Root>
  <Card.Header>
    <Card.Title id="settings-title">
      <div class="flex flex-row items-center justify-items-start gap-2">
        <Settings aria-hidden={true} size={25} />
        <h2>Settings</h2>
      </div>
    </Card.Title>
  </Card.Header>

  <Card.Content>
    <form
      id="settings-form"
      method="POST"
      action="?/settings"
      aria-labelledby="settings-title"
      use:enhance={() => {
        return async ({ result }) => {
          if (result.type === 'success') {
            toast.success('Successfully changed settings');
            remoteSettings = mapFormResultToState(result);
            await applyAction(result);
          } else if (result.type === 'failure') {
            toast.error('Failed to change settings');
          }
        };
      }}>
      <div class="mb-4">
        Use the following settings to customise your gameplay experience. Some settings may require
        the game to be restarted before they can be applied.
      </div>
      <div class="flex flex-row flex-wrap items-start gap-6">
        <!-- Would be nice to use a snippet here, but you can't bind to snippet props :( -->
        <div class="settings-container">
          <Label for="daily-gifts">Receive daily material gifts</Label>
          <Switch name="dailyGifts" id="daily-gifts" bind:checked={localSettings.dailyGifts} />
        </div>
        <div class="settings-container">
          <Label for="use-legacy-helpers">Use legacy helper system</Label>
          <Switch
            name="useLegacyHelpers"
            id="use-legacy-helpers"
            bind:checked={localSettings.useLegacyHelpers} />
        </div>
      </div>
    </form>
  </Card.Content>
  <Card.Footer>
    <div>
      <Button variant="outline" disabled={!isChanged} onclick={handleReset}>Reset</Button>
      <Button type="submit" form="settings-form" disabled={!isChanged}>Save</Button>
    </div>
  </Card.Footer>
</Card.Root>

<style>
  .settings-container {
    display: flex;
    flex-direction: row-reverse;
    align-items: center;
    gap: calc(var(--spacing) * 2);
  }
</style>
