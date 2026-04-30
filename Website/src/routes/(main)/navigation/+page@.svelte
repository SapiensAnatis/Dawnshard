<script lang="ts">
  import LanguageSelector from '$main/languageSelector.svelte';
  import LogInOrOutButton from '$main/logInOrOutButton.svelte';
  import Routes from '$main/routes.svelte';
  import { Separator } from '$shadcn/components/ui/separator';

  import type { PageProps } from './$types';

  let { data }: PageProps = $props();
</script>

<nav class="flex flex-col justify-center gap-1 p-3">
  <h1 class="mb-4 text-3xl font-bold">Navigation</h1>
  <Routes hasValidJwt={data.hasValidJwt} isAdmin={data.isAdmin} />
  <div class="grow"></div>
  <Separator />
  <div class="mt-4 flex flex-col gap-2 px-2 pb-4">
    <LanguageSelector />
    <LogInOrOutButton hasValidJwt={data.hasValidJwt} />
  </div>
</nav>
