using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using WebApp;
using WebApp.Components.Display;
using WebApp.Components.Filters;
using WebApp.Data;
using WebApp.Data.Mocks;

// Datas e números em português, qualquer que seja a cultura do servidor
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços para renderização no servidor via WebSocket
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Login do painel (monitor.Users): a tela inicial confere usuário e senha e o /account/signin grava o cookie; sem ele, o
// MainLayout volta para a tela inicial. Holding e empresa do usuário vão nas claims (UserClaims)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "etl-monitor";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.LoginPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<SignInTickets>();
builder.Services.AddTransient<MudLocalizer, PortugueseMudLocalizer>();

// Holding e empresa da aba (seletor do topo); a primeira escolha vem de LoggedCompany:DocCompany e LoggedCompany:Name
builder.Services.AddScoped<WebApp.Models.LoggedCompany>();
builder.Services.AddScoped<LoggedCompanyStorage>();

// Último período escolhido, por aba (circuito): acompanha o usuário entre as telas quando a URL não traz ?period=
builder.Services.AddScoped<FilterSession>();

// O painel das empresas da tela: uma só ou, na visão "Todas as empresas" da holding, todas, juntadas aqui
builder.Services.AddScoped<CompanyView>();

// Dados (docs/PLANO-WEBAPP.md, fase 5): com Database:Enabled = true, o banco, com Dapper: painel e cadastro em
// ConnectionStrings:Connection (usuário etl_monitor) e Executar Querys em ConnectionStrings:QueryConnection (etl_query),
// as duas nos User Secrets. Sem a chave, o mock, para mexer no visual sem banco.
if (builder.Configuration.GetValue<bool>("Database:Enabled"))
{
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<WebApp.Data.Sql.MainDatabase>();
    builder.Services.AddScoped<IMonitorRepository, WebApp.Data.Sql.SqlMonitorRepository>();
    builder.Services.AddSingleton<IQueryRunner, WebApp.Data.Sql.SqlQueryRunner>();
    builder.Services.AddSingleton<ICompanyRepository, WebApp.Data.Sql.SqlCompanyRepository>();
    builder.Services.AddSingleton<IUserRepository, WebApp.Data.Sql.SqlUserRepository>();
    builder.Services.AddSingleton<IModuleParametersRepository, WebApp.Data.Sql.SqlModuleParametersRepository>();
    builder.Services.AddSingleton<IModuleCredentialReader, SqlModuleCredentialReader>();
}
else
{
    builder.Services.AddSingleton<IMonitorRepository, MockMonitorRepository>();
    builder.Services.AddSingleton<IQueryRunner, MockQueryRunner>();
    builder.Services.AddSingleton<MockCompanyStore>();
    builder.Services.AddSingleton<ICompanyRepository, MockCompanyRepository>();
    builder.Services.AddSingleton<IUserRepository, MockUserRepository>();
    builder.Services.AddSingleton<IModuleParametersRepository, MockModuleParametersRepository>();
    builder.Services.AddSingleton<IModuleCredentialReader, MockModuleCredentialReader>();
}

// Testar conexão da aba Credenciais (docs/PLANO-TELA-CADASTRO-EMPRESA.md, 4.6.1): chamada de leitura do próprio WebApp
builder.Services.AddHttpClient();
builder.Services.AddScoped<ConnectionTester>();

// Sob Demanda: respostas simuladas até o IOnDemandRunner que chama o WebApi (docs/PLANO-WEBAPP.md, 11.5.6). A
// MockTimeline gera as execuções do mock e é a mesma do painel com mock, para uma chamada virar uma execução do job
builder.Services.AddSingleton<MockTimeline>();
builder.Services.AddSingleton<IOnDemandRunner, MockOnDemandRunner>();

// Assistente de IA (Claude, API da Anthropic): só leitura, chave em Assistant:ApiKey nos User Secrets
builder.Services.AddScoped<AssistantTools>();
builder.Services.AddScoped<ClaudeAssistant>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Endereço que não existe cai na página /not-found, a mesma do roteador
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Arquivos da pasta wwwroot (e do MudBlazor) com endereço versionado pelo conteúdo: o App.razor usa @Assets[...], e o
// navegador baixa o arquivo novo assim que ele muda, sem ficar com o CSS antigo em cache
app.MapStaticAssets();

app.MapAccountEndpoints();

// Mapeia os componentes e define a renderização via WebSocket (Server)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();