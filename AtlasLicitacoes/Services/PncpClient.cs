using AtlasLicitacoes.Models.Pncp;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace AtlasLicitacoes.Services;

public sealed class PncpClient(HttpClient httpClient, IMemoryCache cache)
{
    public Task<PagedResponse<AtaRegistroPrecoDto>> GetAtasAsync(DateTime dataInicial, DateTime dataFinal, int pagina, int tamanhoPagina, string? cnpj = null)
        => GetAsync<AtaRegistroPrecoDto>("/v1/atas", dataInicial, dataFinal, pagina, tamanhoPagina, new Dictionary<string, string?> { ["cnpj"] = cnpj });

    public Task<PagedResponse<AtaRegistroPrecoDto>> GetAtasAtualizacaoAsync(DateTime dataInicial, DateTime dataFinal, int pagina, int tamanhoPagina, string? cnpj = null)
        => GetAsync<AtaRegistroPrecoDto>("/v1/atas/atualizacao", dataInicial, dataFinal, pagina, tamanhoPagina, new Dictionary<string, string?> { ["cnpj"] = cnpj });

    public Task<PagedResponse<ContratacaoPublicacaoDto>> GetContratacoesPublicacaoAsync(DateTime dataInicial, DateTime dataFinal, int pagina, int tamanhoPagina, string codigoModalidadeContratacao, string? uf = null)
        => GetAsync<ContratacaoPublicacaoDto>("/v1/contratacoes/publicacao", dataInicial, dataFinal, pagina, tamanhoPagina,
            new Dictionary<string, string?> { ["codigoModalidadeContratacao"] = codigoModalidadeContratacao, ["uf"] = uf });

    public Task<PagedResponse<ContratacaoPublicacaoDto>> GetContratacoesAtualizacaoAsync(DateTime dataInicial, DateTime dataFinal, int pagina, int tamanhoPagina, string codigoModalidadeContratacao, string? uf = null)
        => GetAsync<ContratacaoPublicacaoDto>("/v1/contratacoes/atualizacao", dataInicial, dataFinal, pagina, tamanhoPagina,
            new Dictionary<string, string?> { ["codigoModalidadeContratacao"] = codigoModalidadeContratacao, ["uf"] = uf });

    private async Task<PagedResponse<T>> GetAsync<T>(string path, DateTime dataInicial, DateTime dataFinal, int pagina, int tamanhoPagina, Dictionary<string, string?> extras)
    {
        var cacheKey = $"pncp:{path}:{dataInicial:yyyy-MM-dd}:{dataFinal:yyyy-MM-dd}:{pagina}:{tamanhoPagina}:{string.Join(';', extras.Select(x => $"{x.Key}={x.Value}"))}";
        if (cache.TryGetValue(cacheKey, out PagedResponse<T>? cached) && cached is not null)
        {
            return cached;
        }

        var query = new Dictionary<string, string?>
        {
            ["dataInicial"] = dataInicial.ToString("yyyy-MM-dd"),
            ["dataFinal"] = dataFinal.ToString("yyyy-MM-dd"),
            ["pagina"] = pagina.ToString(),
            ["tamanhoPagina"] = tamanhoPagina.ToString()
        };

        foreach (var item in extras.Where(x => !string.IsNullOrWhiteSpace(x.Value)))
        {
            query[item.Key] = item.Value;
        }

        var queryString = string.Join("&", query.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value ?? string.Empty)}"));
        using var resp = await httpClient.GetAsync($"{path}?{queryString}");

        if (!resp.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"PNCP retornou {(int)resp.StatusCode}: {await resp.Content.ReadAsStringAsync()}");
        }

        var dto = await resp.Content.ReadFromJsonAsync<PagedResponse<T>>()
                  ?? throw new InvalidOperationException("Resposta inválida do PNCP.");

        cache.Set(cacheKey, dto, TimeSpan.FromMinutes(5));
        return dto;
    }
}
