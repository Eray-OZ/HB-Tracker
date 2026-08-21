using Microsoft.AspNetCore.Mvc;
using HBTracker.Web.Models;
using HBTracker.Scraping.Services;


namespace HBTracker.Web.Controllers;

public class ProductsController : Controller
{

    private readonly HBScraper _scraper;

    public ProductsController(HBScraper scraper)
    {
        _scraper = scraper;
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

        var scrapedProduct = await _scraper.ScrapeProductAsync(productUrl.Url);
        Console.WriteLine(scrapedProduct.ProductName + scrapedProduct.Price);

        return RedirectToAction(nameof(Add));
    }


}
