<script lang="ts">
  import { toast } from 'svelte-sonner';

  import Page from '$lib/components/page.svelte';
  import { impersonationSessionSchema } from '$main/admin/impersonation/impersonationSession.ts';
  import { Button } from '$shadcn/components/ui/button';
  import { Input } from '$shadcn/components/ui/input/index.js';

  import type { PageProps } from './$types';

  const { data }: PageProps = $props();

  let impersonatedViewerId = $state(data.impersonatedViewerId);

  async function handleSubmit(
    event: SubmitEvent & { currentTarget: EventTarget & HTMLFormElement }
  ) {
    event.preventDefault();
    const data = new FormData(event.currentTarget);

    let response;

    try {
      response = await fetch('/api/user/impersonation_session', {
        method: 'PUT',
        body: data
      });

      if (!response.ok) {
        throw new Error('Invalid status code: ' + response.status);
      }
    } catch (error) {
      // eslint-disable-next-line no-console
      console.error(error);
      toast.error('Failed to set impersonation session');

      return;
    }

    const parsedResponse = impersonationSessionSchema.parse(await response.json());
    impersonatedViewerId = parsedResponse.impersonatedViewerId;
  }

  async function handleClear() {
    const data = new FormData();
    data.set('impersonatedViewerId', 'null');

    let response;

    try {
      response = await fetch('/api/user/impersonation_session', {
        method: 'PUT',
        body: data
      });

      if (!response.ok) {
        throw new Error('Invalid status code: ' + response.status);
      }
    } catch (error) {
      // eslint-disable-next-line
      console.error(error);
      toast.error('Failed to set impersonation session');

      return;
    }

    const parsedResponse = impersonationSessionSchema.parse(await response.json());
    impersonatedViewerId = parsedResponse.impersonatedViewerId;
  }
</script>

<Page title="User Impersonation">
  <div class="flex w-[350px] flex-col gap-4">
    <div class="flex flex-col gap-3">
      <p>Current impersonation session: {impersonatedViewerId ?? 'null'}</p>
      <Button
        variant="secondary"
        disabled={!impersonatedViewerId}
        onclick={() => {
          handleClear();
        }}>Clear impersonation</Button>
    </div>
    <hr />
    <form class="flex flex-col gap-3" onsubmit={handleSubmit}>
      <label for="impersonation-viewer-id">Enter a viewer ID to impersonate:</label>
      <Input
        name="impersonatedViewerId"
        id="impersonated-viewer-id"
        required
        type="number"
        min="1" />
      <Button type="submit">Submit</Button>
    </form>
  </div>
</Page>
