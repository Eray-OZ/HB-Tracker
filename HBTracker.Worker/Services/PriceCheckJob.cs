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
            await CheckAndRecordPriceDropAsync(product);
        }



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


    private async Task CheckAndRecordPriceDropAsync(TrackedProduct product)
    {
        ScrapedProduct scrapedProduct = await _scraper.ScrapeProductAsync(product.Url);

        if (scrapedProduct.Price < product.CurrentPrice)
        {
            await _context.PriceHistories.AddAsync(
                new PriceHistory
                {
                    TrackedProductId = product.Id,
                    Price = scrapedProduct.Price,
                    CheckedAt = DateTime.Now
                }
            );
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogInformation("{ProductName} price not dropped.", product.ProductName);
        }
    }


}
