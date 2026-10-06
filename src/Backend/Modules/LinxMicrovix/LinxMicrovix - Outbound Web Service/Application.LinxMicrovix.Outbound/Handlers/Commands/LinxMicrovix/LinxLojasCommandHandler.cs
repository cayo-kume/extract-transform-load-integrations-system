using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands
{
    public class LinxLojasCommandHandler : ILinxLojasCommandHandler
    {
        public string CreateGetRegistersExistsQuery()
        {
            return "SELECT CONCAT('[', EMPRESA, ']', '|', '[', CNPJ_EMP, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxLojas]";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}]
                            ([lastupdateon],[portal],[empresa],[nome_emp],[razao_emp],[cnpj_emp],[inscricao_emp],[endereco_emp],[num_emp],[complement_emp],[bairro_emp],[cep_emp],
                             [cidade_emp],[estado_emp],[fone_emp],[email_emp],[cod_ibge_municipio],[data_criacao_emp],[data_criacao_portal],[sistema_tributacao],[regime_tributario],
                             [area_empresa],[timestamp],[sigla_empresa],[id_classe_fiscal],[centro_distribuicao],[inscricao_municipal_emp],[cnae_emp],[cod_cliente_linx],
                             [tabela_preco_preferencial])
                            Values
                            (@lastupdateon,@portal,@empresa,@nome_emp,@razao_emp,@cnpj_emp,@inscricao_emp,@endereco_emp,@num_emp,@complement_emp,@bairro_emp,@cep_emp,@cidade_emp,@estado_emp,
                             @fone_emp,@email_emp,@cod_ibge_municipio,@data_criacao_emp,@data_criacao_portal,@sistema_tributacao,@regime_tributario,@area_empresa,@timestamp,@sigla_empresa,@id_classe_fiscal,
                             @centro_distribuicao,@inscricao_municipal_emp,@cnae_emp,@cod_cliente_linx,@tabela_preco_preferencial)";
        }
    }
}
