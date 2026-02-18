using System.Text.Json.Serialization;

namespace AtlasLicitacoes.Models.Pncp;

public sealed class PagedResponse<T>
{
    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = [];

    [JsonPropertyName("totalRegistros")]
    public long TotalRegistros { get; set; }

    [JsonPropertyName("totalPaginas")]
    public long TotalPaginas { get; set; }

    [JsonPropertyName("numeroPagina")]
    public long NumeroPagina { get; set; }

    [JsonPropertyName("paginasRestantes")]
    public long PaginasRestantes { get; set; }

    [JsonPropertyName("empty")]
    public bool Empty { get; set; }
}

public sealed class AtaRegistroPrecoDto
{
    [JsonPropertyName("numeroControlePNCPAta")]
    public string? NumeroControlePncpAta { get; set; }

    [JsonPropertyName("numeroAtaRegistroPrecos")]
    public string? NumeroAtaRegistroPrecos { get; set; }

    [JsonPropertyName("anoAta")]
    public int? AnoAta { get; set; }

    [JsonPropertyName("objetoContratacao")]
    public string? ObjetoContratacao { get; set; }

    [JsonPropertyName("cnpjOrgao")]
    public string? CnpjOrgao { get; set; }

    [JsonPropertyName("nomeOrgao")]
    public string? NomeOrgao { get; set; }

    [JsonPropertyName("dataPublicacaoPncp")]
    public DateTimeOffset? DataPublicacaoPncp { get; set; }

    [JsonPropertyName("dataAtualizacao")]
    public DateTimeOffset? DataAtualizacao { get; set; }
}

public sealed class ContratacaoPublicacaoDto
{
    [JsonPropertyName("numeroControlePNCP")]
    public string? NumeroControlePncp { get; set; }

    [JsonPropertyName("objetoCompra")]
    public string? ObjetoCompra { get; set; }

    [JsonPropertyName("orgaoEntidade")]
    public string? OrgaoEntidade { get; set; }

    [JsonPropertyName("uf")]
    public string? Uf { get; set; }

    [JsonPropertyName("municipioNome")]
    public string? MunicipioNome { get; set; }

    [JsonPropertyName("dataPublicacaoPncp")]
    public DateTimeOffset? DataPublicacaoPncp { get; set; }

    [JsonPropertyName("dataAtualizacao")]
    public DateTimeOffset? DataAtualizacao { get; set; }
}
