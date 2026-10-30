using WebApi.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder
    .AddArchitectures()
    .AddServices();

var app = builder.Build();

app.UseApplication();

app.MapControllers();

app.Run();