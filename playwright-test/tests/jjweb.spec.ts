// tests/foo.spec.ts
import { test, expect } from '@playwright/test';

test('jjweb test', async ({ page }) => {
  var urlRoot = 'https://jjazgwaks.germanywestcentral.cloudapp.azure.com';

  await page.goto(urlRoot);
  const title = page.locator('footer');
  await expect(title).toContainText('JJ');

  await page.goto(urlRoot + '/home/test/');
  const api = page.locator('.body-content');
  await expect(api).toContainText('value1');

});