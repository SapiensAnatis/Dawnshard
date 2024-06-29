import type { PlaywrightTestConfig } from '@playwright/test';

const config: PlaywrightTestConfig = {
  webServer: {
    command: 'pnpm run build && pnpm run preview',
    url: "http://host.docker.internal:3001",
    stdout: 'pipe',
    reuseExistingServer: !process.env.CI,
    timeout: 600000
  },
  use: {
    "baseURL": "http://host.docker.internal:3001"
  },
  testDir: 'tests',
  testMatch: /(.+\.)?(test|spec)\.[jt]s/,
  timeout: 600000
};

export default config;
