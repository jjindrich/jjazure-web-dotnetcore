import { defineConfig, devices } from '@playwright/test';

const config: PlaywrightTestConfig = {
    testDir: './tests',
    fullyParallel: true,
    forbidOnly: !!process.env.CI,
    retries: process.env.CI ? 2 : 0,
    workers: process.env.CI ? 1 : undefined,
    reporter: 'html',
    use: {
        trace: 'on-first-retry',
    },
    projects: [
        {
            name: 'Microsoft Edge',
            use: {
                // Supported Microsoft Edge channels are: msedge, msedge-beta, msedge-dev, msedge-canary
                channel: 'msedge',
            },
        },

        {
            name: 'chromium',
            use: { ...devices['Desktop Chrome'] },
        },

        {
            name: 'firefox',
            use: { ...devices['Desktop Firefox'] },
        },

        {
            name: 'webkit',
            use: { ...devices['Desktop Safari'] },
        },
    ],
};

export default config