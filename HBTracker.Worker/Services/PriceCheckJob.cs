using HBTracker.Data.Context;
using HBTracker.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HBTracker.Worker.Services;

public class PriceCheckJob
{

    private readonly HBTrackerDbContext _context;
    private readonly ILogger<PriceCheckJob> _logger;
    public PriceCheckJob(HBTrackerDbContext context, ILogger<PriceCheckJob> logger)
    {
        _context = context;
        _logger = logger;
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
