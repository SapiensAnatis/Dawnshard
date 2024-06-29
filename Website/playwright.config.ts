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
    baseURL: 'http://localhost:3001'
  },
  updateSnapshots: process.env.UPDATE_SNAPSHOTS ? 'all' : 'missing',
  ignoreSnapshots: !process.env.CI,
  testDir: 'tests',
  globalSetup: 'tests/globalSetup.ts',
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
