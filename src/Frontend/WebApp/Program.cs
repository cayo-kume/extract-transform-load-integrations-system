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

// Dados do painel: só o mock até a fase 5 do plano (docs/PLANO-WEBAPP.md), quando entra o MonitorRepository com o banco
// A MockTimeline gera as execuções do mock e é a mesma para o painel e para a tela Sob Demanda
builder.Services.AddSingleton<MockTimeline>();
builder.Services.AddSingleton<IMonitorRepository, MockMonitorRepository>();

// Último período escolhido, por aba (circuito): acompanha o usuário entre as telas quando a URL não traz ?period=
builder.Services.AddScoped<FilterSession>();

// Telas de administrador (Executar Querys e Sob Demanda): acesso pela chave Admin:Enabled em Development
builder.Services.AddSingleton<AdminAccess>();

// Executar Querys: até a fase 5, o executor falso
builder.Services.AddSingleton<IQueryRunner, MockQueryRunner>();

// Sob Demanda: até a fase 5, respostas simuladas
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