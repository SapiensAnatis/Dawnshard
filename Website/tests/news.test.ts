import { devices, expect, test } from '@playwright/test';

import { waitForImagesToLoad } from './util.ts';

test('displays correctly', async ({ page }) => {
  await page.goto('/');

  const newsLink = page.getByRole('link', { name: 'News' });
  await expect(newsLink).toBeVisible();
  await newsLink.click();

  await expect(page.getByRole('heading', { name: 'News' })).toBeVisible();

  await waitForImagesToLoad(page);

  await expect(page).toHaveScreenshot();
});

test('displays correctly on mobile', async ({ page }) => {
  await page.setViewportSize(devices['iPhone 13'].viewport);

  await page.goto('/');

  await page.getByRole('button', { name: 'Open navigation' }).click();
  await page.getByRole('link', { name: 'News' }).click();

  await expect(page.getByRole('heading', { name: 'News' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Game updated!' })).toBeVisible();

  await page.waitForFunction(() => {
    const images = Array.from(document.querySelectorAll('img'));
    return images.every((img) => img.complete);
  });

  await expect(page).toHaveScreenshot();
});

test('change page', async ({ page }) => {
  await page.goto('/news');

  await page.getByRole('link', { name: 'Next' }).click();

  await expect(page.getByRole('heading', { name: 'News' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Paging works' })).toBeVisible();

  await page.getByRole('link', { name: 'Previous' }).click();

  await expect(page.getByRole('heading', { name: 'News' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Game updated!' })).toBeVisible();
});

test.describe('webview', () => {
  test('displays correctly', async ({ page }) => {
    await page.setViewportSize(devices['iPhone 13'].viewport);

    await page.goto('/webview/news');

    await expect(page.getByRole('heading', { name: 'Welcome to Dawnshard' })).toBeVisible();

    await page.waitForFunction(() => {
      const images = Array.from(document.querySelectorAll('img'));
      return images.every((img) => img.complete);
    });

    await expect(page).toHaveScreenshot();
  });

  test('change page', async ({ page }) => {
    await page.goto('/webview/news');

    await page.getByRole('link', { name: 'Next' }).click();

    await expect(page.getByRole('heading', { name: 'Paging works' })).toBeVisible();

    await page.getByRole('link', { name: 'Previous' }).click();

    await expect(page.getByRole('heading', { name: 'Game updated!' })).toBeVisible();
  });

  test('open details', async ({ page }) => {
    await page.goto('/webview/news');

    await expect(page.getByRole('heading', { name: 'Game updated!' })).toBeVisible();

    await page.getByRole('link', { name: 'Game updated!' }).click();

    await expect(page).toHaveURL(/.*webview\/news\/detail\/6/);

    await expect(page.getByRole('heading', { name: 'Game updated!' })).toBeVisible();
    await expect(page.getByText('We have done a very large update!')).toBeVisible();

    await page.getByRole('link', { name: 'Back to news' }).click();

    expect(page).toHaveURL(/.*webview\/news/);

    await expect(page.getByRole('heading', { name: 'Really long story' })).toBeVisible();
  });
});
