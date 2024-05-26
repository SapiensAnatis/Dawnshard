<script lang="ts">
  import { Avatar, AvatarImage } from '$shadcn/components/ui/avatar/index.js';

  export let avatarSrc: string;
  export let name: string;
</script>

<li class="my-2 flex items-center gap-3">
  <Avatar>
    <AvatarImage src={avatarSrc} alt={`${name} avatar image`} />
  </Avatar>
  <p><strong>{name}</strong>, <slot /></p>
</li>
