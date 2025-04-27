import { type Page, test } from '@playwright/test';

export const waitForImagesToLoad = async (page: Page) => {
  await test.step('wait for images to load', async () => {
    await page.waitForFunction(() => {
      const images = Array.from(document.querySelectorAll('img'));
      return images.every((img) => img.complete);
    });
  });
};

export const gotoWithHydration = async (
  page: Page,
  url: string,
  opts?: { waitForStarted?: boolean }
) => {
  await page.goto(url);
  if (opts?.waitForStarted !== false) {
    await page.waitForSelector('body.started', { timeout: 5000 });
  }
};
