<script lang="ts">
  import { invalidate } from '$app/navigation';
  import { locale, locales, translations } from '$lib/translations';

  function change(e: Event) {
    const next = (e.currentTarget as HTMLSelectElement).value;
    document.cookie = `locale=${next}; path=/; max-age=31536000; SameSite=Lax`;
    invalidate('app:locale');
  }
</script>

<select
  aria-label="Language"
  bind:value={$locale}
  onchange={change}
  class="border-input bg-background rounded-md border px-2 text-sm">
  {#each $locales as l (l)}
    <option value={l}>{translations[l] ?? l}</option>
  {/each}
</select>
