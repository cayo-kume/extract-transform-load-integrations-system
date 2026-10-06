using Domain.Core.Entities.Base;

namespace Domain.LinxCommerce.Entities.Parameters
{
    public class LinxCommerceJobParameter : ParameterBase
    {
        public string? schema { get; private set; }
        public string? databaseName { get; private set; }
        public string? untreatedDatabaseName { get; private set; }
        public string? jobName { get; private set; }
        public string? tableName { get; private set; }
        public string? parametersInterval { get; private set; }

        /// <summary>
        /// Set parameters for any job
        /// </summary>
        /// <param name="docMainCompany"></param>
        /// <param name="schema"></param>
        /// <param name="databaseName"></param>
        /// <param name="untreatedDatabaseName"></param>
        /// <param name="projectName"></param>
        /// <param name="keyAuthorization"></param>
        /// <param name="userAuthentication"></param>
        /// <param name="parametersInterval"></param>
        /// <param name="parametersTableName"></param>
        public LinxCommerceJobParameter(
            string? docMainCompany,
            string? schema,
            string? databaseName,
            string? untreatedDatabaseName,
            string? projectName,
            string? keyAuthorization,
            string? userAuthentication,
            string? parametersInterval,
            string? parametersTableName
        ) : base(
            docMainCompany: docMainCompany,
            projectName: projectName,
            keyAuthorization: keyAuthorization,
            userAuthentication: userAuthentication,
            parametersTableName: parametersTableName
        )
        {
            this.schema = schema;
            this.databaseName = databaseName;
            this.untreatedDatabaseName = untreatedDatabaseName;
            this.parametersInterval = parametersInterval;
        }

        /// <summary>
        /// Set parameters for an specific job
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public LinxCommerceJobParameter SetParameters(string? jobName, string? tableName)
        {
            this.jobName = jobName;
            this.tableName = tableName;

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="parametersInterval"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public LinxCommerceJobParameter SetParameters(string? jobName, string? parametersInterval, string? tableName)
        {
            this.jobName = jobName;
            this.tableName = tableName;
            this.parametersInterval = parametersInterval;

            return this;
        }
    }
}
