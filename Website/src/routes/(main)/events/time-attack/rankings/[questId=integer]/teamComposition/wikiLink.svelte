<script lang="ts">
  const { pageName }: { pageName: string } = $props();

  let href = $derived(`https://dragalialost.wiki/w/${pageName.replaceAll(' ', '_')}`);
</script>

<!--
  This is an external link so it's not appropriate to use resolve()
  See: https://github.com/sveltejs/eslint-plugin-svelte/issues/1454
-->
<!--eslint-disable-next-line svelte/no-navigation-without-resolve -->
<a class="link text-sm" target="_blank" {href} rel="external">Dragalia Lost Wiki</a>
