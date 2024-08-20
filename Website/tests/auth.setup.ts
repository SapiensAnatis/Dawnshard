import { expect, test as setup } from '@playwright/test';

const authFile = 'playwright/.auth/user.json';

setup('authenticate', async ({ page }) => {
  if (!process.env.BAAS_USERNAME || !process.env.BAAS_PASSWORD) {
    throw new Error(
      'All tests need the environment variables BAAS_USERNAME and BAAS_PASSWORD to be set.'
    );
  }

  await page.goto('/');

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

  await expect(page.getByRole('heading', { name: 'Dawnshard', exact: true })).toBeVisible();

  await page.context().storageState({ path: authFile });
});
