import { devices, expect, test } from '@playwright/test';

import { waitForImagesToLoad } from './util.ts';

test('displays correctly', async ({ page }) => {
  await page.goto('/');

  await page.getByRole('link', { name: 'Time Attack Rankings' }).click();

  await expect(page.getByRole('heading', { name: 'Time Attack Rankings' })).toBeVisible();

  await waitForImagesToLoad(page);

  await expect(page).toHaveScreenshot();
});

test('displays correctly on mobile', async ({ page }) => {
  await page.setViewportSize(devices['iPhone 13'].viewport);

  await page.goto('/');

  await page.getByRole('button', { name: 'Open navigation' }).click();
  await page.getByRole('link', { name: 'Time Attack Rankings' }).click();

  await expect(page.getByRole('heading', { name: 'Time Attack Rankings' })).toBeVisible();

  await waitForImagesToLoad(page);

  await expect(page).toHaveScreenshot({ fullPage: true });
});

test('expand team composition', async ({ page }) => {
  await page.goto('/events/time-attack/rankings/227010105');

  const topRow = page.getByRole('row', { name: /1 Qwerby/ });

  await page.waitForTimeout(500); // evil random wait otherwise expanding row doesn't work in CI?

  await topRow.getByRole('button', { name: 'View detailed team information' }).click();
  await expect(page.getByRole('button', { name: 'Expand dragon details' }).first()).toBeVisible();
  await waitForImagesToLoad(page);

  await expect(page).toHaveScreenshot({ fullPage: true });
});

test('clicking icons shows info popovers', async ({ page }) => {
  await page.goto('/events/time-attack/rankings/227010105');

  await waitForImagesToLoad(page);

  await page.waitForTimeout(500); // ???

  const topRow = page.getByRole('row', { name: /1 Qwerby/ });

  await topRow.getByRole('button', { name: 'Expand character details' }).first().click();
  await expect(page.getByText('Sheila')).toBeVisible();
  const charaWikiLink = page.getByText('Dragalia Lost Wiki');
  await expect(charaWikiLink).toBeVisible();
  await expect(charaWikiLink).toHaveAttribute('href', 'https://dragalialost.wiki/w/Sheila');
});
