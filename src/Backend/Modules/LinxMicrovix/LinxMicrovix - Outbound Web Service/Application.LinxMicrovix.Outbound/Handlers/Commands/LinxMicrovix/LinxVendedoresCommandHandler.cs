using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxVendedoresCommandHandler : ILinxVendedoresCommandHandler
    {
        public string CreateGetRegistersExistsQuery()
        {
            return "SELECT CONCAT('[', COD_VENDEDOR, ']', '|', '[', CPF_VENDEDOR, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxVendedores]";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO {tableName}_raw 
                          ([lastupdateon], [portal], [cod_vendedor], [nome_vendedor], [tipo_vendedor], [end_vend_rua], [end_vend_numero], [end_vend_complemento], [end_vend_bairro], [end_vend_cep], [end_vend_cidade], [end_vend_uf], [fone_vendedor], [mail_vendedor], [dt_upd], [cpf_vendedor], [ativo], [data_admissao], [data_saida], [timestamp], [matricula], [id_tipo_venda], [descricao_tipo_venda], [cargo]) 
                          Values 
                          (@lastupdateon, @portal, @cod_vendedor, @nome_vendedor, @tipo_vendedor, @end_vend_rua, @end_vend_numero, @end_vend_complemento, @end_vend_bairro, @end_vend_cep, @end_vend_cidade, @end_vend_uf, @fone_vendedor, @mail_vendedor, @dt_upd, @cpf_vendedor, @ativo, @data_admissao, @data_saida, @timestamp, @matricula, @id_tipo_venda, @descricao_tipo_venda, @cargo)";
        }
    }
}
