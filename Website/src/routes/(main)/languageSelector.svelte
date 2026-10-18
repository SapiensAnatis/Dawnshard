<script lang="ts">
  import { get, translations } from '$lib/translations';

  const i18n = get();

  function change(e: Event) {
    const next = (e.currentTarget as HTMLSelectElement).value;
    document.cookie = `locale=${next}; path=/; max-age=31536000; SameSite=Lax`;
    i18n.locale = next;
  }
</script>

<select
  aria-label="Language"
  value={i18n.locale}
  onchange={change}
  class="border-input bg-background rounded-md border px-2 text-sm">
  {#each i18n.locales as l (l)}
    <option value={l}>{translations[l] ?? l}</option>
  {/each}
</select>
