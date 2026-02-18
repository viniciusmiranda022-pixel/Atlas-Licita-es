using AtlasLicitacoes.Data;
using AtlasLicitacoes.Models.ViewModels;
using AtlasLicitacoes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AtlasLicitacoes.Controllers;

public sealed class PortfolioController(AppDbContext db, PortfolioCsvImporter importer) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var query = db.PortfolioProducts.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.Sku.Contains(search) || x.Description.Contains(search) || x.ProductLine.Contains(search));
        }

        var items = await query.OrderBy(x => x.ProductLine).Take(300)
            .Select(x => new PortfolioCardVm
            {
                Vendor = x.Vendor,
                ProductLine = x.ProductLine,
                Sku = x.Sku,
                Description = x.Description,
                LicensingType = x.LicensingType,
                Price = x.Price
            }).ToListAsync();

        return View(new PortfolioPageVm { Search = search, Itens = items });
    }

    [HttpPost]
    public async Task<IActionResult> ImportCsv(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            TempData["Status"] = "Selecione um CSV válido.";
            return RedirectToAction(nameof(Index));
        }

        await using var stream = file.OpenReadStream();
        var count = await importer.ImportAsync(stream);
        TempData["Status"] = $"{count} itens processados.";
        return RedirectToAction(nameof(Index));
    }
}
