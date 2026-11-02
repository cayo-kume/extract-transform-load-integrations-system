using System.Globalization;
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
builder.Services.AddTransient<MudLocalizer, PortugueseMudLocalizer>();

// Empresa logada (LoggedCompany:DocCompany e LoggedCompany:Name): fixa por configuração até a fase 6 (autenticação)
WebApp.Models.LoggedCompany.Configure(builder.Configuration);

// Último período escolhido, por aba (circuito): acompanha o usuário entre as telas quando a URL não traz ?period=
builder.Services.AddScoped<FilterSession>();

// Telas de administrador (Executar Querys e Sob Demanda): acesso pela chave Admin:Enabled em Development
builder.Services.AddSingleton<AdminAccess>();

// Dados (docs/PLANO-WEBAPP.md, fase 5): com Database:Enabled = true, o banco, com Dapper: painel e cadastro em
// ConnectionStrings:Connection (usuário etl_monitor) e Executar Querys em ConnectionStrings:QueryConnection (etl_query),
// as duas nos User Secrets. Sem a chave, o mock, para mexer no visual sem banco.
if (builder.Configuration.GetValue<bool>("Database:Enabled"))
{
    builder.Services.AddMemoryCache();
    builder.Services.AddSingleton<WebApp.Data.Sql.MainDatabase>();
    builder.Services.AddSingleton<IMonitorRepository, WebApp.Data.Sql.SqlMonitorRepository>();
    builder.Services.AddSingleton<IQueryRunner, WebApp.Data.Sql.SqlQueryRunner>();
    builder.Services.AddSingleton<ICompanyRepository, WebApp.Data.Sql.SqlCompanyRepository>();
    builder.Services.AddSingleton<IModuleParametersRepository, WebApp.Data.Sql.SqlModuleParametersRepository>();
}
else
{
    builder.Services.AddSingleton<IMonitorRepository, MockMonitorRepository>();
    builder.Services.AddSingleton<IQueryRunner, MockQueryRunner>();
    builder.Services.AddSingleton<MockCompanyStore>();
    builder.Services.AddSingleton<ICompanyRepository, MockCompanyRepository>();
    builder.Services.AddSingleton<IModuleParametersRepository, MockModuleParametersRepository>();
}

// Sob Demanda: respostas simuladas até o IOnDemandRunner que chama o WebApi (docs/PLANO-WEBAPP.md, 11.5.6). A
// MockTimeline gera as execuções do mock e é a mesma do painel com mock, para uma chamada virar uma execução do job
builder.Services.AddSingleton<MockTimeline>();
builder.Services.AddSingleton<IOnDemandRunner, MockOnDemandRunner>();

// Assistente de IA (Claude, API da Anthropic): só leitura, chave em Assistant:ApiKey nos User Secrets
builder.Services.AddSingleton<AssistantTools>();
builder.Services.AddSingleton<ClaudeAssistant>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Endereço que não existe cai na página /not-found, a mesma do roteador
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

// Arquivos da pasta wwwroot (e do MudBlazor) com endereço versionado pelo conteúdo: o App.razor usa @Assets[...], e o
// navegador baixa o arquivo novo assim que ele muda, sem ficar com o CSS antigo em cache
app.MapStaticAssets();

// Mapeia os componentes e define a renderização via WebSocket (Server)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();