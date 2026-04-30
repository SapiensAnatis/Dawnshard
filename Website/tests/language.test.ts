import { devices, expect, test } from '@playwright/test';

import { gotoWithHydration } from './util.ts';

test('desktop selector renders with correct options', async ({ page }) => {
  await page.goto('/');

  const selector = page.getByRole('combobox', { name: 'Language' });
  await expect(selector).toBeVisible();
  await expect(selector.getByRole('option', { name: 'English' })).toBeAttached();
  await expect(selector.getByRole('option', { name: '中文 (partial)' })).toBeAttached();
});

test('mobile drawer contains selector', async ({ page }) => {
  await page.setViewportSize(devices['iPhone 13'].viewport);
  await page.goto('/');

  await page.getByRole('button', { name: 'Open navigation' }).click();

  await expect(page.getByRole('combobox', { name: 'Language' })).toBeVisible();
});

test('selecting a language sets the locale cookie', async ({ page }) => {
  await page.goto('/');

  await page.getByRole('combobox', { name: 'Language' }).selectOption('zh-CN');

  await page.waitForFunction(async () => {
    const cookies = document.cookie;
    return cookies.includes('locale=zh-CN');
  });
});

test('pre-set locale cookie is respected', async ({ page }) => {
  await page
    .context()
    .addCookies([{ name: 'locale', value: 'zh-CN', url: 'http://localhost:3001' }]);
  await page.goto('/');

  await expect(page.getByRole('combobox', { name: 'Language' })).toHaveValue('zh-CN');
});

test('Accept-Language header drives locale detection', async ({ page, context }) => {
  await context.route('**/*', (route, request) => {
    route.continue({
      headers: {
        ...request.headers(),
        'accept-language': 'zh-CN,de;q=0.9,en;q=0.8'
      }
    });
  });

  await page.goto('/');

  await expect(page.getByRole('combobox', { name: 'Language' })).toHaveValue('zh-CN');
});

test('Accept-Language header uses fallback', async ({ page, context }) => {
  await context.route('**/*', (route, request) => {
    route.continue({
      headers: {
        ...request.headers(),
        'accept-language': 'zh-TW'
      }
    });
  });

  await page.goto('/');

  await expect(page.getByRole('combobox', { name: 'Language' })).toHaveValue('zh-CN');
});

test('localises save editor', async ({ page }) => {
  await gotoWithHydration(page, '/account/save-editor');

  const languageSelect = page.getByRole('combobox', { name: 'Language' });
  await languageSelect.selectOption('zh-CN');

  const giftBoxForm = page.getByRole('form', { name: 'Gift box' });

  const typeSelect = giftBoxForm.getByRole('combobox', { name: 'Type' });
  await typeSelect.selectOption('Material');
  await expect(page.locator('#type option:checked')).toHaveText('材料');

  await page.getByRole('combobox', { name: 'Language' }).selectOption('en');

  await typeSelect.selectOption('Material');
  await expect(page.locator('#type option:checked')).toHaveText('Material');
});
