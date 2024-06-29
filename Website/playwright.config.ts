import { devices, type PlaywrightTestConfig } from '@playwright/test';

const url = process.env.CI ? 'http://localhost:4173' : 'http://localhost:3001';

const config: PlaywrightTestConfig = {
  webServer: {
    command: 'pnpm run build && pnpm run preview -- --mode dev',
    url,
    stdout: 'pipe',
    reuseExistingServer: !process.env.CI,
    timeout: 120000
  },
  use: {
    baseURL: url
  },
  updateSnapshots: process.env.UPDATE_SNAPSHOTS ? 'all' : 'missing',
  ignoreSnapshots: !process.env.CI,
  testDir: 'tests',
  snapshotPathTemplate: '{testDir}/__screenshots__/{testFilePath}/{arg}{ext}',
  testMatch: /(.+\.)?(test|spec)\.[jt]s/,
  timeout: 60000,
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] }
    }
  ]
};

export default config;
