using System.Runtime.InteropServices;
using HBTracker.Data.Context;
using HBTracker.Data.Entities;
using HBTracker.Scraping.Models;
using HBTracker.Scraping.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HBTracker.Worker.Services;

public class PriceCheckJob
{

    private readonly HBTrackerDbContext _context;
    private readonly ILogger<PriceCheckJob> _logger;
    private readonly HBScraper _scraper;
    public PriceCheckJob(HBTrackerDbContext context, ILogger<PriceCheckJob> logger, HBScraper scraper)
    {
        _context = context;
        _logger = logger;
        _scraper = scraper;
    }



    public async Task RunAsync()
    {
        List<TrackedProduct> products =
            await LoadActiveProductsAsync();

        _logger.LogInformation(
            "Found {Count} active tracked products.",
            products.Count);

        foreach (TrackedProduct product in products)
        {
            _logger.LogInformation(
                "Product: {ProductName} | Seller: {SellerName} | URL: {Url}",
                product.ProductName,
                product.SellerName ?? "Unknown",
                product.Url);
        }


        var scrapedProduct = await _scraper.ScrapeProductAsync("https://www.hepsiburada.com/apple-iphone-15-128-gb-mavi-p-HBCV00004X9ZCK");
        Console.WriteLine($"Name: {scrapedProduct.ProductName}");
        Console.WriteLine($"Price: {scrapedProduct.Price}");
        Console.WriteLine($"URL: {scrapedProduct.Url}");
    }



    private Task<List<TrackedProduct>> LoadActiveProductsAsync()
    {
        CancellationToken cancellationToken = default;
        var products = _context.TrackedProducts
        .Where(p => p.IsActive)
        .AsNoTracking()
        .ToListAsync(cancellationToken);
        return products;
    }


}
