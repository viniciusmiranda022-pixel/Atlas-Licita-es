using AtlasLicitacoes.Data;
using AtlasLicitacoes.Models.Portfolio;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AtlasLicitacoes.Services;

public sealed class PortfolioCsvImporter(AppDbContext db)
{
    public async Task<int> ImportAsync(Stream csvStream, CancellationToken ct = default)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            BadDataFound = null,
            MissingFieldFound = null,
            HeaderValidated = null
        });

        var rows = csv.GetRecords<PortfolioCsvRow>().ToList();
        var count = 0;

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Sku))
            {
                continue;
            }

            var existing = await db.PortfolioProducts.FirstOrDefaultAsync(x => x.Sku == row.Sku, ct);
            if (existing is null)
            {
                existing = new PortfolioProduct { Sku = row.Sku.Trim() };
                db.PortfolioProducts.Add(existing);
            }

            existing.Vendor = string.IsNullOrWhiteSpace(row.Vendor) ? "Microsoft" : row.Vendor.Trim();
            existing.ProductLine = row.ProductLine?.Trim() ?? string.Empty;
            existing.Description = row.Description?.Trim() ?? string.Empty;
            existing.LicensingType = row.LicensingType?.Trim() ?? string.Empty;
            existing.Price = row.Price;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
            count++;
        }

        await db.SaveChangesAsync(ct);
        return count;
    }

    private sealed record PortfolioCsvRow
    {
        public string? Vendor { get; init; }
        public string? ProductLine { get; init; }
        public string? Sku { get; init; }
        public string? Description { get; init; }
        public string? LicensingType { get; init; }
        public decimal? Price { get; init; }
    }
}
