using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using WebApp;
using WebApp.Data;
using WebApp.Data.Mocks;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços para renderização no servidor via WebSocket
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Dados do painel: só o mock até a fase 5 do plano (docs/PLANO-WEBAPP.md), quando entra o MonitorRepository com o banco
builder.Services.AddSingleton<IMonitorRepository, MockMonitorRepository>();

// Executar Querys: acesso pela chave QueryRunner:Enabled em Development e, até a fase 5, o executor falso
builder.Services.AddSingleton<QueryRunnerAccess>();
builder.Services.AddSingleton<IQueryRunner, MockQueryRunner>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Endereço que não existe cai na página /not-found, a mesma do roteador
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Habilita o uso da pasta wwwroot
app.UseStaticFiles();
app.UseAntiforgery();

// Mapeia os componentes e define a renderização via WebSocket (Server)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();