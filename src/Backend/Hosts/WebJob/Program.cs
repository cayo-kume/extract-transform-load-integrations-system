using Hosts.Scheduling.RecurringJobs;
using WebJob.Extensions;
using WebJob.Scheduling;

// WebJob.exe --list-jobs: IdJob, título e versão de cada job dos mapas, um por linha e separados por tabulação, sem subir o
// host nem acessar o banco. É o que o script de publicação lê para montar o nome de cada WebJob (<Title> - V<versão>)
if (args.Contains("--list-jobs"))
{
    foreach (var definition in WebJobRecurringJobs.Definitions.Values.OrderBy(d => d.IdJob))
        Console.WriteLine($"{definition.IdJob}\t{definition.Title}\t{definition.Version}");

    return;
}

var builder = Host.CreateApplicationBuilder(args);

// WebJob.exe --job 8006: o job que este processo roda (cada WebJob na Azure roda um job só)
builder.Configuration.AddCommandLine(args, new Dictionary<string, string> { ["--job"] = $"{WebJobSettings.SectionName}:IdJob" });

builder.AddServices();

builder.Services.Configure<WebJobSettings>(builder.Configuration.GetSection(WebJobSettings.SectionName));
builder.Services.AddHostedService<JobScheduleWorker>();

var host = builder.Build();
host.Run();
