import { test, expect } from '@playwright/test';

test('jjweb test', async ({ page }) => {
  var urlRoot = 'https://jjazgwaks.germanywestcentral.cloudapp.azure.com';

  await page.goto(urlRoot);

  // if you comment, the test will fail
  const consent = page.locator("#cookieConsent")
  if (consent){
    await expect(consent).toContainText('privacy');
    await page.getByRole('button', { name: 'Accept' }).click();  
  }

  const title = page.locator('footer');
  await expect(title).toContainText('JJ');

  await page.getByRole('link', { name: 'Test' }).click();

  const api = page.locator('.body-content');
  await expect(api).toContainText('value1');

});