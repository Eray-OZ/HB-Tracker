using Microsoft.AspNetCore.Mvc;
using HBTracker.Web.Models;
using HBTracker.Scraping.Services;
using HBTracker.Data.Context;
using HBTracker.Data.Entities;
using HBTracker.Scraping.Models;

namespace HBTracker.Web.Controllers;

public class ProductsController : Controller
{

    private readonly HBScraper _scraper;
    private readonly HBTrackerDbContext _context;

    public ProductsController(HBScraper scraper, HBTrackerDbContext context)
    {
        _scraper = scraper;
        _context = context;
    }


    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Add(AddProductViewModel productUrl)
    {
        if (!ModelState.IsValid)
        {
            return View(productUrl);
        }

        ScrapedProduct scrapedProduct;
        try { scrapedProduct = await _scraper.ScrapeProductAsync(productUrl.Url); }

        catch 
        {    
            return View(productUrl);
        }

        await _context.TrackedProducts.AddAsync(new TrackedProduct
        {
            ProductName = scrapedProduct.ProductName,
            CurrentPrice = scrapedProduct.Price,
            Url = productUrl.Url,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Add));
    }


}
