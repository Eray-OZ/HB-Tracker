using HBTracker.Scraping.Models;
using Microsoft.Playwright;

namespace HBTracker.Scraping.Services;

public class HBScraper
{
    public async Task<ScrapedProduct> ScrapeProductAsync(string url)
    {
        using IPlaywright playwright =
            await Playwright.CreateAsync();

        await using IBrowser browser =
            await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Channel = "chrome",
                    Args = new[] {
                    "--disable-blink-features=AutomationControlled",
                }
                });


        var context = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36",
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            Locale = "tr-TR",
            TimezoneId = "Europe/Istanbul"
        });


        IPage page = await context.NewPageAsync();

        await page.GotoAsync(
            url,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });


        ILocator productHeading = page.Locator("h1").First;
        await productHeading.WaitForAsync(
            new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });
        var name = await productHeading.InnerTextAsync();

        ILocator productPrice = page.Locator("div[data-test-id='default-price']");
        await productPrice.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
        var priceString = await productPrice.InnerTextAsync();
        string cleanPrice = priceString.Replace("TL", "").Trim();
        var turkishCulture = new System.Globalization.CultureInfo("tr-TR");
        decimal priceDecimal = decimal.Parse(cleanPrice, turkishCulture);


        return new ScrapedProduct
        {
            ProductName = name,
            Price = priceDecimal,
            Url = url
        };

    }
}