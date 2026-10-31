using WebJob.Extensions;
using WebJob.Scheduling;

var builder = Host.CreateApplicationBuilder(args);

// WebJob.exe --job 8006: o job que este processo roda (cada WebJob na Azure roda um job só)
builder.Configuration.AddCommandLine(args, new Dictionary<string, string> { ["--job"] = $"{WebJobSettings.SectionName}:IdJob" });

builder.AddServices();

builder.Services.Configure<WebJobSettings>(builder.Configuration.GetSection(WebJobSettings.SectionName));
builder.Services.AddHostedService<JobScheduleWorker>();

var host = builder.Build();
host.Run();
