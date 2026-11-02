using Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServices();
builder.AddQuartzService();

var host = builder.Build();
host.Run();
