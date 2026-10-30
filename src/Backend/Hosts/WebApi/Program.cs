using WebApi.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);
var serverName = builder.Configuration.GetSection("ConfigureServer").GetSection("ServerName").Value;

ConfigurationHelperExtensions.Initialize(builder.Configuration);

builder
    .AddArchitectures()
    .AddServices();

var app = builder.Build();

app.UseApplication(serverName);

app.MapControllers();

app.Run();