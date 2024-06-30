import { expect, type Page, test } from '@playwright/test';

test('displays correctly', async ({ page }) => {
  await page.goto('/');

  await login(page);

  const profileLink = page.getByRole('link', { name: 'Profile' });
  await expect(profileLink).toBeVisible();
  await profileLink.click();

  await expect(page.getByRole('heading', { name: 'Profile' })).toBeVisible();

  await expect(page).toHaveScreenshot();
});

test('export save', async ({ page }) => {
  await page.goto('/');

  await login(page);

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

const login = async (page: Page) => {
  if (!process.env.BAAS_USERNAME || !process.env.BAAS_PASSWORD) {
    throw new Error(
      'This test needs the environment variables BAAS_USERNAME and BAAS_PASSWORD to be set.'
    );
  }

  await page.getByRole('link', { name: 'Login' }).click();

  // Begin baas.lukefz.xyz
  await page.waitForTimeout(5000); // If removed, buttons don't work. Maybe need to wait for Blazor to initialize?
  await page.getByText('Login', { exact: true }).click();

  await page.locator('css=#username').fill(process.env.BAAS_USERNAME);
  await page.locator('css=#password').fill(process.env.BAAS_PASSWORD);
  await page.getByRole('button', { name: 'Login' }).click();

  await expect(page.getByRole('heading', { name: 'Login using Dragalia' })).toBeVisible();
  await page.getByRole('button', { name: 'Login' }).click();
  // End baas.lukefz.xyz
};
