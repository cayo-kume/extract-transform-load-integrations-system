using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using WebApp;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços para renderização no servidor via WebSocket
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Aqui você deve adicionar os serviços que antes vinham da sua API
// (ex: builder.Services.AddDbContext<SeuContexto>(...))
// (ex: builder.Services.AddScoped<ISeuServico, SeuServico>())
builder.Services.AddMudServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

// Habilita o uso da pasta wwwroot
app.UseStaticFiles();
app.UseAntiforgery();

// Mapeia os componentes e define a renderização via WebSocket (Server)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();