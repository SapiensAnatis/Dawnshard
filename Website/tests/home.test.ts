import { devices, expect, test } from '@playwright/test';

test('displays correctly', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'Dawnshard', exact: true })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Welcome to Dawnshard' })).toBeVisible();

  await expect(page).toHaveScreenshot({ fullPage: true });
});

test('displays correctly in dark mode', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'Dawnshard', exact: true })).toBeVisible();
  await expect(page.getByRole('button', { name: 'Toggle theme' })).toHaveAttribute(
    'data-loaded',
    'true'
  );
  await page.getByRole('button', { name: 'Toggle theme' }).click();
  await expect(page.getByRole('document')).toHaveAttribute('style', 'color-scheme: dark;');

  await expect(page).toHaveScreenshot();
});

test('displays correctly on mobile', async ({ page }) => {
  await page.setViewportSize(devices['iPhone 13'].viewport);

  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'Dawnshard', exact: true })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Welcome to Dawnshard' })).toBeVisible();

  await expect(page).toHaveScreenshot();
});
