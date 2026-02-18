# Atlas Licitações (PNCP + Portfólio Microsoft)

MVP em **ASP.NET Core (.NET 8, MVC + Bootstrap 5)** para:

1. Consultar PNCP em:
   - `GET /v1/atas`
   - `GET /v1/atas/atualizacao`
   - `GET /v1/contratacoes/publicacao`
   - `GET /v1/contratacoes/atualizacao`
2. Detectar automaticamente oportunidades de **licenciamento Microsoft** (Windows, Windows Server, SQL Server, CAL, RDS etc.) com dicionário configurável.
3. Manter cache/histórico local em banco SQLite (fácil para MVP; pode migrar para SQL Server).
4. Oferecer um segundo módulo para **portfólio Ingram** via importação CSV.

## Estrutura principal

- `AtlasLicitacoes/Program.cs`: DI, HTTP client, cache, MVC, DB, background sync.
- `AtlasLicitacoes/Services/PncpClient.cs`: integração com os endpoints PNCP e paginação.
- `AtlasLicitacoes/Services/SearchClassifier.cs`: normalização de texto e classificação por keywords.
- `AtlasLicitacoes/Services/PncpSyncBackgroundService.cs`: sincronização incremental automática.
- `AtlasLicitacoes/Controllers/LicitacoesController.cs`: UI de oportunidades (Atas + Contratações).
- `AtlasLicitacoes/Controllers/PortfolioController.cs`: UI de portfólio e importação CSV.

## Publicação no IIS (Windows)

1. Instale .NET 8 SDK e ASP.NET Core Hosting Bundle no servidor.
2. `dotnet publish -c Release -o .\publish`
3. Crie um site/app no IIS apontando para `publish`.
4. Configure `appsettings.Production.json` com URL/token do PNCP.
5. Garanta permissão de escrita para o arquivo SQLite (`atlas.db`) no diretório da aplicação.

## CSV de portfólio

Delimitador esperado: `;`
Colunas esperadas:

- `Vendor`
- `ProductLine`
- `Sku`
- `Description`
- `LicensingType`
- `Price`

## Próximos passos (fase 2)

- Trocar SQLite por SQL Server.
- Adicionar Hangfire para jobs e monitoramento.
- Integrar API da Ingram Micro com credenciais.
- Incluir tela administrativa para keywords e watchlist avançada.
