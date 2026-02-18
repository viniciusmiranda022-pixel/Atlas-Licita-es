namespace AtlasLicitacoes.Models.Portfolio;

public sealed class PortfolioProduct
{
    public int Id { get; set; }
    public string Vendor { get; set; } = "Microsoft";
    public string ProductLine { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LicensingType { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
