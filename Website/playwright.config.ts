import { devices, type PlaywrightTestConfig } from '@playwright/test';

const config: PlaywrightTestConfig = {
  webServer: {
    command: 'pnpm run build:dev && pnpm run preview',
    url: 'http://localhost:3001',
    stdout: 'pipe',
    reuseExistingServer: !process.env.CI,
    timeout: 120000
  },
  use: {
    baseURL: 'http://localhost:3001',
    bypassCSP: true
  },
  updateSnapshots: process.env.UPDATE_SNAPSHOTS ? 'all' : 'missing',
  ignoreSnapshots: !process.env.CI,
  testDir: 'tests',
  globalSetup: 'tests/globalSetup.ts',
  snapshotPathTemplate: '{testDir}/__screenshots__/{testFilePath}/{arg}{ext}',
  testMatch: /(.+\.)?(test|spec)\.[jt]s/,
  timeout: 15000,
  projects: [
    {
      name: 'setup',
      testMatch: /.*\.setup\.ts/
    },
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'], storageState: 'playwright/.auth/user.json' },
      dependencies: ['setup']
    }
  ],
  expect: {
    toHaveScreenshot: {
      maxDiffPixelRatio: 0.01
    }
  }
};

export default config;
