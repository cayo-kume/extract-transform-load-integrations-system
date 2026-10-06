//using Domain.LinxMicrovix.Outbound.WebService.Entities.Parameters;

namespace Hangfire.Extensions.RecurringJobs
{
    public static class LinxMicrovixERPRecurringJobsExtensions
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
            //    .GetSection("LinxMicrovix")
            //    .GetSection("DocMainCompany")
            //    .Value;

            //_databaseName = ConfigurationHelperExtensions.config
            //    .GetSection("ConfigureServer")
            //    .GetSection("BLOOMERS_LINX")
            //    .Value;

            //_projectName = ConfigurationHelperExtensions.config
            //    .GetSection("LinxMicrovix")
            //    .GetSection("ProjectName")
            //    .Value;

            //_parametersLogTableName = ConfigurationHelperExtensions.config
            //    .GetSection("LinxMicrovix")
            //    .GetSection("ProjectParametersLogTableName")
            //    .Value;

            //_parametersTableName = ConfigurationHelperExtensions.config
            //    .GetSection("LinxMicrovix")
            //    .GetSection("ProjectParametersTableName")
            //    .Value;

            //_key = ConfigurationHelperExtensions.config
            //    .GetSection("LinxMicrovix")
            //    .GetSection("Key")
            //    .Value;

            //_authentication = ConfigurationHelperExtensions.config
            //    .GetSection("LinxMicrovix")
            //    .GetSection("Authentication")
            //    .Value;

            //_parametersInterval = ConfigurationHelperExtensions.config
            //                .GetSection("LinxMicrovix")
            //                .GetSection("ParametersDateInterval")
            //                .Value;

            //_methods = ConfigurationHelperExtensions.config
            //                .GetSection("LinxMicrovix")
            //                .GetSection("Methods")
            //                .Get<List<LinxMethods>>();

            //if (serverName == "SRV-VM-APP02")
            //{
            //    //SELECIONAR JOBS PARA O SERVIDOR SRV-VM-APP02 RODAR
            //    LinxMicrovixRecurringJobs();
            //}
            //else if (serverName == "SRV-VM-APP01")
            //{
            //    //SELECIONAR JOBS PARA O SERVIDOR SRV-VM-APP01 RODAR
            //}
            //else
            //{
            //    //TESTE PARA SERVIDORES SEM NOME
            //    FlashCourierRecurringJobs();
            //}
        }

        private static void LinxMicrovixRecurringJobs()
        {
            //RecurringJob.AddOrUpdate<ILinxProdutosCodBarService>("LinxProdutosCodBar", service => service.GetRecords(
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
            //                .Where(m => m.MethodName == "LinxProdutosCodBar")
            //                .FirstOrDefault().MethodName,
            //    tableName = _methods
            //                .Where(m => m.MethodName == "LinxProdutosCodBar")
            //                .FirstOrDefault().MethodName
            //}
            //),
            //Cron.MinuteInterval(3)
            //, queue: "srv-vm-app02"
            //);
        }

        //private static void FlashCourierRecurringJobs()
        //{
        //    RecurringJob.AddOrUpdate<IFlashCourierService>("FlashCourierSendOrders", service => service.SendOrders(),
        //        Cron.MinuteInterval(3)
        //    //,queue: "srv-vm-app02"
        //    );

        //    RecurringJob.AddOrUpdate<IFlashCourierService>("FlashCourierUpdateLogOrders", service => service.UpdateLogOrders(),
        //        Cron.MinuteInterval(5)
        //    //,queue: "srv-vm-app02"
        //    );
        //}
    }
}
