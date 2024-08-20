import { expect, type Page, test } from '@playwright/test';

test('displays correctly', async ({ page }) => {
  await page.goto('/');

  const profileLink = page.getByRole('link', { name: 'Profile' });
  await expect(profileLink).toBeVisible();
  await profileLink.click();

  await expect(page.getByRole('heading', { name: 'Profile' })).toBeVisible();

  await expect(page).toHaveScreenshot();
});

test('export save', async ({ page }) => {
  await page.goto('/');

  const profileLink = page.getByRole('link', { name: 'Profile' });
  await expect(profileLink).toBeVisible();
  await profileLink.click();

  await expect(page.getByRole('heading', { name: 'Profile' })).toBeVisible();

  await page.getByRole('button', { name: 'Export Save' }).click();

  const downloadPromise = page.waitForEvent('download');
  const download = await downloadPromise;

  // @ts-expect-error Stream reading hack; clearly not the intended use of the Response API, but it works :)
  const text = await new Response(await download.createReadStream()).text();

  expect(text).toBe('{"someData":"true"}');
});
