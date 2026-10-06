using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands
{
    public class LinxGrupoLojasCommandHandler : ILinxGrupoLojasCommandHandler
    {
        public string CreateGetRegistersExistsQuery()
        {
            return "SELECT CONCAT('[', CNPJ, ']', '|', '[', NOME_EMPRESA, ']', '|', '[', ID_EMPRESAS_REDE, ']', '|', '[', REDE, ']', '|', '[', PORTAL, ']', '|', '[', NOME_PORTAL, ']' , '|', '[', EMPRESA, ']', '|', '[', CLASSIFICACAO_PORTAL, ']') FROM [linx_microvix_erp].[LinxGrupoLojas]";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}]
                            ([lastupdateon],[cnpj],[nome_empresa],[id_empresas_rede],[rede],[portal],[nome_portal],[empresa],[classificacao_portal])
                            Values
                            (@lastupdateon,@cnpj,@nome_empresa,@id_empresas_rede,@rede,@portal,@nome_portal,@empresa,@classificacao_portal)";
        }
    }
}
