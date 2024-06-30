import { devices, expect, test } from '@playwright/test';

test('displays correctly', async ({ page }) => {
  await page.goto('/');

  const newsLink = page.getByRole('link', { name: 'News' });
  await expect(newsLink).toBeVisible();
  await newsLink.click();

  await expect(page.getByRole('heading', { name: 'News' })).toBeVisible();

  await expect(page).toHaveScreenshot();
});

test('displays correctly on mobile', async ({ page }) => {
  await page.setViewportSize(devices['iPhone 13'].viewport);

  await page.goto('/');

  await page.getByRole('button', { name: 'Open navigation' }).click();
  await page.getByRole('link', { name: 'News' }).click();

  await expect(page.getByRole('heading', { name: 'News' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Game updated!' })).toBeVisible();

  await expect(page).toHaveScreenshot();
});

test('change page', async ({ page }) => {
  await page.goto('/news/1');

  await page.getByRole('link', { name: 'Next' }).click();

  await expect(page.getByRole('heading', { name: 'News' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Paging works' })).toBeVisible();

  await page.getByRole('link', { name: 'Previous' }).click();

  await expect(page.getByRole('heading', { name: 'News' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Game updated!' })).toBeVisible();
});

test('displays correctly for webview', async ({ page }) => {
  await page.setViewportSize(devices['iPhone 13'].viewport);

  await page.goto('/webview/news/1');

  await expect(page.getByRole('heading', { name: 'Welcome to Dawnshard' })).toBeVisible();

  await expect(page).toHaveScreenshot();
});
