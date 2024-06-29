<script lang="ts">
  import Info from 'lucide-svelte/icons/info';

  import * as Card from '$shadcn/components/ui/card';

  import type { User } from '../user.ts';
  import type { UserProfile } from './userProfile.ts';

  const chunkString = (s: string, chunkSize: number) => {
    const chunks = [];
    for (let i = 0; i < s.length; i += chunkSize) {
      chunks.push(s.substring(i, i + chunkSize));
    }

    return chunks;
  };

  const formatViewerId = (id: number) => {
    const paddedId = id.toString().padStart(11, '0');
    const chunkedId = chunkString(paddedId, 4);
    return chunkedId.join(' ');
  };

  const renderDate = (date: Date) => {
    if (date.valueOf() < new Date(1970, 1, 1).valueOf()) {
      return 'Never!';
    }

    return date.toLocaleString(undefined, {
      dateStyle: 'medium',
      timeStyle: 'short'
    });
  };

  export let user: User;
  export let userProfile: UserProfile;
</script>

<Card.Root>
  <Card.Header>
    <Card.Title>
      <div class="flex flex-row items-center justify-items-start gap-2">
        <Info size={25} />
        <h2 class="m-0 text-xl font-bold">User information</h2>
      </div>
    </Card.Title>
  </Card.Header>
  <Card.Content>
    <div class="flex flex-col gap-4">
      <div>
        <p class="font-semibold">Viewer ID</p>
        <p>{formatViewerId(user.viewerId)}</p>
      </div>
      <div>
        <p class="font-semibold">Player name</p>
        <p>{user.name}</p>
      </div>
      <div>
        <p class="font-semibold">Last login time</p>
        <p>{renderDate(userProfile.lastLoginTime)}</p>
      </div>
    </div>
  </Card.Content>
</Card.Root>
