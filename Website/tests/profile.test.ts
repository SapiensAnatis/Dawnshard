import { devices, expect, test } from '@playwright/test';

import { gotoWithHydration } from './util.ts';

test('displays correctly', async ({ page }) => {
  await page.goto('/');

  const profileLink = page.getByRole('link', { name: 'Profile' });
  await expect(profileLink).toBeVisible();
  await profileLink.click();

  await expect(page.getByRole('heading', { name: 'Profile' })).toBeVisible();

  await expect(page).toHaveScreenshot();
});

test('displays correctly on mobile', async ({ page }) => {
  await page.setViewportSize(devices['iPhone 13'].viewport);

  await page.goto('/');

  await page.getByRole('button', { name: 'Open navigation' }).click();
  await page.getByRole('link', { name: 'Profile' }).click();

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

test('change settings', async ({ page }) => {
  await gotoWithHydration(page, '/account/profile');

  await expect(page.getByRole('heading', { name: 'Settings' })).toBeVisible();
  await expect(page.getByRole('form', { name: 'Settings' })).toBeVisible();

  const form = page.getByRole('form', { name: 'Settings' });

  await expect(form.getByRole('switch', { name: 'Receive daily material gifts' })).toBeEnabled();
  await expect(form.getByRole('switch', { name: 'Receive daily material gifts' })).toBeChecked(); // Default is enabled
  await expect(form.getByRole('button', { name: 'Reset' })).toBeDisabled();
  await expect(form.getByRole('button', { name: 'Save' })).toBeDisabled();

  // Submit
  await form.getByRole('switch', { name: 'Receive daily material gifts' }).click();
  await expect(
    form.getByRole('switch', { name: 'Receive daily material gifts' })
  ).not.toBeChecked();
  await expect(form.getByRole('button', { name: 'Save' })).toBeEnabled();
  await form.getByRole('button', { name: 'Save' }).click();
  await expect(page.getByText('Successfully changed settings')).toBeVisible();

  // Reset
  await form.getByRole('switch', { name: 'Receive daily material gifts' }).click();
  await expect(form.getByRole('switch', { name: 'Receive daily material gifts' })).toBeChecked();
  await expect(form.getByRole('button', { name: 'Reset' })).toBeEnabled();
  await form.getByRole('button', { name: 'Reset' }).click();
  await expect(
    form.getByRole('switch', { name: 'Receive daily material gifts' })
  ).not.toBeChecked();
  await expect(form.getByRole('button', { name: 'Reset' })).toBeDisabled();
  await expect(form.getByRole('button', { name: 'Save' })).toBeDisabled();
});
