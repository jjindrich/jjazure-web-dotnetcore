// tests/foo.spec.ts
import { test, expect } from '@playwright/test';

test('jjweb test', async ({ page }) => {
  await page.goto('https://jjazaks.westeurope.cloudapp.azure.com/');
  const title = page.locator('footer');
  await expect(title).toContainText('JJ');

  await page.goto('https://jjazaks.westeurope.cloudapp.azure.com/home/test/');
  const api = page.locator('.body-content');
  await expect(api).toContainText('value1');

});