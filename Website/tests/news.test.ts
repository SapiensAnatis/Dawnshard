import { devices, expect, test } from '@playwright/test';

test('displays correctly', async ({ page }) => {
  await page.goto('/');

  const newsLink = page.getByRole('link', { name: 'News' });
  await expect(newsLink).toBeVisible();
  await newsLink.click();

  await expect(page.getByRole('heading', { name: 'News' })).toBeVisible();

  await expect(page).toHaveScreenshot({ fullPage: true });
});

test('displays correctly on mobile', async ({ page }) => {
  await page.setViewportSize(devices['iPhone 13'].viewport);

  await page.goto('/');

  await page.getByRole('button', { name: 'Open navigation' }).click();
  await page.getByRole('link', { name: 'News' }).click();

  await expect(page.getByRole('heading', { name: 'News' })).toBeVisible();

  await expect(page).toHaveScreenshot({ fullPage: true });
});
