<script lang="ts">
  export let pageName: string;

  $: href = `https://dragalialost.wiki/w/${pageName.replaceAll(' ', '_')}`;
</script>

<a class="link text-sm" target="_blank" {href}>Dragalia Lost Wiki</a>
