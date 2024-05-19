<script lang="ts">
  import { onMount } from 'svelte';
  import { browser } from '$app/environment';
  import { goto } from '$app/navigation';

  onMount(() => {
    let destination = '/';
    if (browser) {
      destination = localStorage.getItem('originalPage') ?? '/';
    }

    goto(destination);
  });
</script>

<p>Authenticating...</p>
