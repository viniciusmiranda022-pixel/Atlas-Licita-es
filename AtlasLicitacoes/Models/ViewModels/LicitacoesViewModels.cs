namespace AtlasLicitacoes.Models.ViewModels;

public sealed class OpportunitiesFilterVm
{
    public DateTime DataInicial { get; set; } = DateTime.Today.AddDays(-7);
    public DateTime DataFinal { get; set; } = DateTime.Today;
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
    public string? Cnpj { get; set; }
    public string? Uf { get; set; }
    public string CodigoModalidadeContratacao { get; set; } = "1";
    public string? BuscaLivre { get; set; }
}

public sealed class OpportunityCardVm
{
    public string SourceType { get; set; } = string.Empty;
    public string? NumeroControle { get; set; }
    public string? Orgao { get; set; }
    public string? Objeto { get; set; }
    public DateTimeOffset? Publicacao { get; set; }
    public List<string> Matches { get; set; } = [];
}

public sealed class OpportunitiesPageVm
{
    public OpportunitiesFilterVm Filtros { get; set; } = new();
    public long TotalPaginas { get; set; }
    public List<OpportunityCardVm> Itens { get; set; } = [];
}

public sealed class PortfolioPageVm
{
    public string? Search { get; set; }
    public List<PortfolioCardVm> Itens { get; set; } = [];
}

public sealed class PortfolioCardVm
{
    public string Vendor { get; set; } = string.Empty;
    public string ProductLine { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LicensingType { get; set; } = string.Empty;
    public decimal? Price { get; set; }
}
