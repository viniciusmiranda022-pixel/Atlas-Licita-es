using AtlasLicitacoes.Data;
using AtlasLicitacoes.Models.ViewModels;
using AtlasLicitacoes.Services;
using Microsoft.AspNetCore.Mvc;

namespace AtlasLicitacoes.Controllers;

public sealed class LicitacoesController(PncpClient pncpClient, SearchClassifier classifier, AppDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Atas([FromQuery] OpportunitiesFilterVm filtro)
    {
        var dto = await pncpClient.GetAtasAsync(filtro.DataInicial, filtro.DataFinal, filtro.Pagina, filtro.TamanhoPagina, filtro.Cnpj);
        var itens = dto.Data
            .Select(x => new OpportunityCardVm
            {
                SourceType = "Ata",
                NumeroControle = x.NumeroControlePncpAta,
                Orgao = x.NomeOrgao,
                Objeto = x.ObjetoContratacao,
                Publicacao = x.DataPublicacaoPncp,
                Matches = classifier.GetMatches(x.ObjetoContratacao)
            })
            .Where(x => x.Matches.Any())
            .ToList();

        return View("Opportunities", new OpportunitiesPageVm { Filtros = filtro, Itens = itens, TotalPaginas = dto.TotalPaginas });
    }

    [HttpGet]
    public async Task<IActionResult> Contratacoes([FromQuery] OpportunitiesFilterVm filtro)
    {
        var dto = await pncpClient.GetContratacoesPublicacaoAsync(
            filtro.DataInicial,
            filtro.DataFinal,
            filtro.Pagina,
            filtro.TamanhoPagina,
            filtro.CodigoModalidadeContratacao,
            filtro.Uf);

        var itens = dto.Data
            .Select(x => new OpportunityCardVm
            {
                SourceType = "Contratação",
                NumeroControle = x.NumeroControlePncp,
                Orgao = x.OrgaoEntidade,
                Objeto = x.ObjetoCompra,
                Publicacao = x.DataPublicacaoPncp,
                Matches = classifier.GetMatches(x.ObjetoCompra)
            })
            .Where(x => x.Matches.Any())
            .ToList();

        return View("Opportunities", new OpportunitiesPageVm { Filtros = filtro, Itens = itens, TotalPaginas = dto.TotalPaginas });
    }

    [HttpPost]
    public async Task<IActionResult> Watchlist(string sourceKey, string notes)
    {
        db.Watchlist.Add(new WatchlistItem { SourceKey = sourceKey, Notes = notes });
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Atas));
    }
}
