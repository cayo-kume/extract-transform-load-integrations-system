using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands
{
    public class LinxMicrovixCommandHandler : ILinxMicrovixCommandHandler
    {
        public string CreateGetParametersQuery(string parametersInterval, string parametersTableName, string jobName)
        {
            return $"SELECT {parametersInterval} FROM [LINX_MICROVIX].[{parametersTableName}] (NOLOCK) WHERE METHOD = '{jobName}'";
        }

        public string CreateGetB2CCompanysQuery()
        {
            return @"SELECT  
                       EMPRESA AS Code,
                       CNPJ_EMP AS Doc,
                       NOME_EMP AS Reason,
                       NOME_EMP AS Name,
                       EMAIL_UNIDADE AS Email,
                       END_UNIDADE AS Street,
                       NR_RUA_UNIDADE AS StreetNumber,
                       COMPLEMENTO_END_UNIDADE AS ComplementAddress,
                       BAIRRO_UNIDADE AS Neighborhood,
                       CIDADE_UNIDADE AS City,
                       UF_UNIDADE AS State,
                       CEP_UNidade AS ZipCode,
                       '' AS Phone,
                       '' AS StateRegistration,
                       '' AS MunicipalRegistration
                       FROM 
                       [LINX_MICROVIX_COMMERCE].[B2CCONSULTAEMPRESAS] (NOLOCK)";
        }

        public string CreateGetMicrovixCompanysQuery()
        {
            return @"SELECT  
                       EMPRESA AS Code,
                       CNPJ_EMP AS Doc,
                       RAZAO_EMP AS Reason,
                       NOME_EMP AS Name,
                       INSCRICAO_EMP AS StateRegistration,
                       EMAIL_EMP AS Email,
                       ENDERECO_EMP AS Street,
                       NUM_EMP AS StreetNumber,
                       COMPLEMENT_EMP AS ComplementAddress,
                       BAIRRO_EMP AS Neighborhood,
                       CIDADE_EMP AS City,
                       ESTADO_EMP AS State,
                       CEP_EMP AS ZipCode,
                       FONE_EMP AS Phone,
                       INSCRICAO_MUNICIPAL_EMP AS MunicipalRegistration
                       FROM 
                       [LINX_MICROVIX_ERP].[LINXLOJAS] (NOLOCK)";
        }

        public string CreateGetMicrovixGroupCompanysQuery()
        {
            return @"SELECT  
                       EMPRESA AS Code,
                       CNPJ AS Doc,
                       NOME_EMPRESA AS Reason,
                       NOME_EMPRESA AS Name
                       FROM 
                       [LINX_MICROVIX_ERP].[LINXGRUPOLOJAS] (NOLOCK)
                       WHERE CNPJ <> ''";
        }

        public string CreateGetLast7DaysMaxTimestampQuery(string schema, string tableName)
        {
            return $"SELECT ISNULL(MAX(TIMESTAMP), 0) FROM [{schema}].[{tableName}] (NOLOCK)";
        }

        public string CreateGetLastMaxTimestampQuery(string schema, string tableName, string columnCompany, string companyValue)
        {
            return $"SELECT ISNULL(MAX(TIMESTAMP), 0) FROM [{schema}].[{tableName}] (NOLOCK) WHERE [{columnCompany}] = '{companyValue}'";
        }

        public string CreateGetLastMaxTimestampByCnpjAndIdentificadorQuery(string schema, string tableName, string columnCompany, string companyValue, string columnIdentificador, string columnIdentificadorValue)
        {
            return $"SELECT ISNULL(MAX(TIMESTAMP), 0) FROM [{schema}].[{tableName}] (NOLOCK) WHERE [{columnCompany}] = '{companyValue}' AND [{columnIdentificador}] = '{columnIdentificadorValue}'";
        }

        public string CreateGetLast7DaysMinTimestampQuery(string schema, string tableName, string columnDate)
        {
            return $"SELECT ISNULL(MIN(TIMESTAMP), 0) FROM [{schema}].[{tableName}] (NOLOCK) WHERE {columnDate} > GETDATE() - 7";
        }

        public string CreateGetLast7DaysMinTimestampWithCompanyQuery(string schema, string tableName, string columnDate, string columnCompany, string companyValue)
        {
            return $"SELECT ISNULL(MIN(TIMESTAMP), 0) FROM [{schema}].[{tableName}] (NOLOCK) WHERE {columnDate} > GETDATE() - 7 AND {columnCompany} = '{companyValue}'";
        }
    }
}
