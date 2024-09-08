<script lang="ts">
  import BannerDarkWide from '$lib/assets/bannerDark-wide.webp?enhanced';
  import BannerLightWide from '$lib/assets/bannerLight-wide.webp?enhanced';
</script>

<picture class="background-image">
  {#each Object.entries(BannerDarkWide.sources) as [format, source]}
    <source
      class="background-image"
      srcset={source}
      type={`image/${format}`}
      media="(prefers-color-scheme: dark)" />
  {/each}
  {#each Object.entries(BannerLightWide.sources) as [format, source]}
    <source
      class="background-image"
      srcset={source}
      type={`image/${format}`}
      media="(prefers-color-scheme: light)" />
  {/each}
  <img
    class="background-image"
    src={BannerDarkWide.img.src}
    width={BannerDarkWide.img.w}
    height={BannerDarkWide.img.h}
    alt="Welcome to Dawnshard banner background"
    loading="eager"
    fetchpriority="high" />
</picture>

<style>
  .background-image {
    object-fit: cover;
    object-position: 10% 20%;
    pointer-events: none;
    position: absolute;
    width: 100%;
    height: 100%;
    z-index: -1;
  }

  @media (max-width: 1080px) {
    .background-image {
      object-position: 60% 0;
    }
  }
</style>
