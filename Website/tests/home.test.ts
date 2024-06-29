import { devices, expect, test } from '@playwright/test';

test('displays correctly', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'Dawnshard', exact: true })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Welcome to Dawnshard' })).toBeVisible();

  await expect(page).toHaveScreenshot({ fullPage: true });
});

test('displays correctly on mobile', async ({ page }) => {
  await page.setViewportSize(devices['iPhone 13'].viewport);

  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'Dawnshard', exact: true })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Welcome to Dawnshard' })).toBeVisible();

  await expect(page).toHaveScreenshot();
});
