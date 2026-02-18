using AtlasLicitacoes.Data;
using AtlasLicitacoes.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PncpOptions>(builder.Configuration.GetSection("Pncp"));
builder.Services.Configure<SearchRuleOptions>(builder.Configuration.GetSection("SearchRules"));

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=atlas.db"));

builder.Services.AddHttpClient<PncpClient>((sp, http) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["Pncp:BaseUrl"] ?? throw new InvalidOperationException("Pncp:BaseUrl não configurado.");
    http.BaseAddress = new Uri(baseUrl);
    http.Timeout = TimeSpan.FromSeconds(30);

    var token = cfg["Pncp:BearerToken"];
    if (!string.IsNullOrWhiteSpace(token))
    {
        http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
});

builder.Services.AddSingleton<SearchClassifier>();
builder.Services.AddScoped<PortfolioCsvImporter>();
builder.Services.AddHostedService<PncpSyncBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Licitacoes}/{action=Atas}/{id?}");

app.Run();
