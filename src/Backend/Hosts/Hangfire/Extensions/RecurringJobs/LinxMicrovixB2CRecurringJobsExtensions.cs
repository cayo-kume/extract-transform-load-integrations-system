//using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Hangfire.Extensions.RecurringJobs
{
    public static class LinxMicrovixB2CRecurringJobsExtensions
    {
        private static string? _docMainCompany;
        private static string? _databaseName;
        private static string? _projectName;
        private static string? _parametersInterval;
        private static string? _parametersTableName;
        private static string? _parametersLogTableName;
        private static string? _key;
        private static string? _authentication;
        //private static List<LinxMethods>? _methods;

        public static void AddRecurringJobs()
        {
            //_docMainCompany = ConfigurationHelperExtensions.config
            //    .GetSection("B2CLinxMicrovix")
            //    .GetSection("DocMainCompany")
            //    .Value;

            //_databaseName = ConfigurationHelperExtensions.config
            //    .GetSection("ConfigureServer")
            //    .GetSection("LinxMicrovixCommerceDatabaseName")
            //    .Value;

            //_projectName = ConfigurationHelperExtensions.config
            //    .GetSection("B2CLinxMicrovix")
            //    .GetSection("ProjectName")
            //    .Value;

            //_parametersLogTableName = ConfigurationHelperExtensions.config
            //    .GetSection("B2CLinxMicrovix")
            //    .GetSection("ProjectParametersLogTableName")
            //    .Value;

            //_parametersTableName = ConfigurationHelperExtensions.config
            //    .GetSection("B2CLinxMicrovix")
            //    .GetSection("ProjectParametersTableName")
            //    .Value;

            //_key = ConfigurationHelperExtensions.config
            //    .GetSection("B2CLinxMicrovix")
            //    .GetSection("Key")
            //    .Value;

            //_authentication = ConfigurationHelperExtensions.config
            //    .GetSection("B2CLinxMicrovix")
            //    .GetSection("Authentication")
            //    .Value;

            //_parametersInterval = ConfigurationHelperExtensions.config
            //                .GetSection("B2CLinxMicrovix")
            //                .GetSection("ParametersDateInterval")
            //                .Value;

            //_methods = ConfigurationHelperExtensions.config
            //                .GetSection("B2CLinxMicrovix")
            //                .GetSection("Methods")
            //                .Get<List<LinxMethods>>();

            //RecurringJob.AddOrUpdate<IB2CConsultaClientesService>("B2CConsultaClientes", service => service.GetRecords(
            //new LinxAPIParam
            //{
            //    docMainCompany = _docMainCompany,
            //    databaseName = _databaseName,
            //    projectName = _projectName,
            //    keyAuthorization = _key,
            //    userAuthentication = _authentication,
            //    parametersTableName = _parametersTableName,
            //    parametersLogTableName = _parametersLogTableName,
            //    parametersInterval = _parametersInterval,
            //    jobName = _methods
            //                .Where(m => m.MethodName == "B2CConsultaClientes")
            //                .FirstOrDefault().MethodName,
            //    tableName = _methods
            //                .Where(m => m.MethodName == "B2CConsultaClientes")
            //                .FirstOrDefault().MethodName
            //}
            //),
            //Cron.MinuteInterval(3), 
            //queue: "srv-vm-app02"
            //);
        }
    }
}
