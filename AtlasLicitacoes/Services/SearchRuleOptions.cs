namespace AtlasLicitacoes.Services;

public sealed class SearchRuleOptions
{
    public List<string> Keywords { get; set; } =
    [
        "windows 11", "windows 10", "windows server", "server 2022", "server 2019", "sql server",
        "microsoft sql", "cal", "client access license", "rds", "rds cal", "remote desktop",
        "microsoft", "licenca", "licenciamento", "subscription", "software"
    ];
}
