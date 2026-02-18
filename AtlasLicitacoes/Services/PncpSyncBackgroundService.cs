using AtlasLicitacoes.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AtlasLicitacoes.Services;

public sealed class PncpSyncBackgroundService(IServiceProvider provider, ILogger<PncpSyncBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = provider.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<PncpClient>();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var classifier = scope.ServiceProvider.GetRequiredService<SearchClassifier>();

                var end = DateTime.Today;
                var start = end.AddDays(-2);

                var atas = await client.GetAtasAtualizacaoAsync(start, end, 1, 100);
                foreach (var item in atas.Data)
                {
                    var tags = classifier.GetMatches(item.ObjetoContratacao);
                    if (!tags.Any())
                    {
                        continue;
                    }

                    var key = item.NumeroControlePncpAta ?? Guid.NewGuid().ToString("N");
                    var existing = await db.Opportunities.FirstOrDefaultAsync(x => x.SourceKey == key, stoppingToken);
                    if (existing is null)
                    {
                        existing = new OpportunityCache { SourceKey = key, SourceType = "Ata" };
                        db.Opportunities.Add(existing);
                    }

                    existing.Title = item.NumeroAtaRegistroPrecos ?? key;
                    existing.Description = item.ObjetoContratacao ?? string.Empty;
                    existing.Organization = item.NomeOrgao ?? string.Empty;
                    existing.PublishedAt = item.DataPublicacaoPncp;
                    existing.UpdatedAt = item.DataAtualizacao;
                    existing.MatchTags = string.Join(", ", tags);
                    existing.RawJson = JsonSerializer.Serialize(item);
                    existing.SyncedAt = DateTimeOffset.UtcNow;
                }

                await db.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Falha na sincronização automática com o PNCP.");
            }

            await Task.Delay(TimeSpan.FromMinutes(20), stoppingToken);
        }
    }
}
