import { devices, expect, test } from '@playwright/test';
import { list } from 'postcss';

test('displays correctly', async ({ page }) => {
  await page.goto('/');

  const profileLink = page.getByRole('link', { name: 'Save Editor' });
  await expect(profileLink).toBeVisible();
  await profileLink.click();

  await expect(page.getByRole('heading', { name: 'Save Editor' })).toBeVisible();

  await expect(page).toHaveScreenshot();
});

test('displays correctly on mobile', async ({ page }) => {
  await page.setViewportSize(devices['iPhone 13'].viewport);

  await page.goto('/');

  await page.getByRole('button', { name: 'Open navigation' }).click();
  await page.getByRole('link', { name: 'Save Editor' }).click();

  await expect(page.getByRole('heading', { name: 'Save Editor' })).toBeVisible();

  await expect(page).toHaveScreenshot();
});

test('edit save and submit', async ({ page }) => {
  await page.goto('/account/save-editor');

  const giftBoxForm = page.getByRole('form', { name: 'Gift box' });
  await expect(giftBoxForm).toBeVisible();

  const submitButton = giftBoxForm.getByRole('button', { name: 'Add' });
  await expect(submitButton).toBeDisabled();

  await page.waitForTimeout(500); // otherwise the form screws up?

  const typeSelect = giftBoxForm.getByRole('combobox', { name: 'Type' });
  await typeSelect.selectOption({ label: 'Material' });
  await typeSelect.press('Tab');

  const itemSelect = giftBoxForm.getByRole('combobox', { name: 'Item' });
  await itemSelect.selectOption({ label: 'Gold Crystal' });
  await itemSelect.press('Tab');

  const quantityInput = giftBoxForm.getByRole('spinbutton', { name: 'Quantity' });
  await quantityInput.fill('999');
  await quantityInput.press('Tab');

  await submitButton.click();

  const stagedChanges = page.getByRole('list', { name: 'Staged changes' });
  await expect(stagedChanges).toBeVisible();

  const listItem = stagedChanges.getByRole('listitem');
  await expect(listItem).toBeVisible();
  await expect(listItem).toHaveText('Add 999x Gold Crystal to the gift box');

  await page.getByRole('button', { name: 'Save changes' }).click();

  await expect(page.getByRole('status')).toHaveText('Successfully edited save');

  await expect(listItem).not.toBeVisible();
});
