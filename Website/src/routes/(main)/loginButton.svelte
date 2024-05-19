<script lang="ts">
  import { page } from '$app/stores';
  import { Button } from '$shadcn/components/ui/button';
</script>

<Button href="/login" on:click={() => localStorage.setItem('originalPage', $page.url.pathname)}
  >Login</Button
>
