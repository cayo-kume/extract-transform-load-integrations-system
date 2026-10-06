using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands
{
    public class B2CConsultaClientesCommandHandler : IB2CConsultaClientesCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<string> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', COD_CLIENTE_B2C, ']', '|', '[', COD_CLIENTE_ERP, ']', '|', '[', DOC_CLIENTE, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_commerce].[B2CCONSULTACLIENTES] WHERE DOC_CLIENTE IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].{tableName} 
                          ([lastupdateon], [cod_cliente_b2c], [cod_cliente_erp], [doc_cliente], [nm_cliente], [nm_mae], [nm_pai], [nm_conjuge], [dt_cadastro], 
                          [dt_nasc_cliente], [end_cliente], [complemento_end_cliente], [nr_rua_cliente], [bairro_cliente], [cep_cliente], [cidade_cliente], 
                          [uf_cliente], [fone_cliente], [fone_comercial], [cel_cliente], [email_cliente], [rg_cliente], [rg_orgao_emissor], [estado_civil_cliente], 
                          [empresa_cliente], [cargo_cliente], [sexo_cliente], [dt_update], [ativo], [receber_email], [dt_expedicao_rg], [naturalidade], [tempo_residencia], 
                          [renda], [numero_compl_rua_cliente], [timestamp], [tipo_pessoa], [portal], [aceita_programa_fidelidade])
                          Values 
                          (@lastupdateon, @cod_cliente_b2c, @cod_cliente_erp, @doc_cliente, @nm_cliente, @nm_mae, @nm_pai, @nm_conjuge, @dt_cadastro, 
                          @dt_nasc_cliente, @end_cliente, @complemento_end_cliente, @nr_rua_cliente, @bairro_cliente, @cep_cliente, @cidade_cliente, 
                          @uf_cliente, @fone_cliente, @fone_comercial, @cel_cliente, @email_cliente, @rg_cliente, @rg_orgao_emissor, @estado_civil_cliente, 
                          @empresa_cliente, @cargo_cliente, @sexo_cliente, @dt_update, @ativo, @receber_email, @dt_expedicao_rg, @naturalidade, @tempo_residencia, 
                          @renda, @numero_compl_rua_cliente, @timestamp, @tipo_pessoa, @portal, @aceita_programa_fidelidade)";
        }

        public string CreateIntegrityLockQuery()
        {
            return @$"select distinct top 500 
                      'B2CConsultaClientes' as [table], 
                      'doc_cliente' as recordKey, 
                      IIF(a.cpf is null, a.cnpj, a.cpf) as identifier 
                      from linx_commerce.customer a (nolock)
                      left join linx_microvix_commerce.B2CConsultaClientes b (nolock) on IIF(a.cpf is null, a.cnpj, a.cpf) = b.doc_cliente
                      left join linx_microvix.IntegrityLockTablesRegisters c (nolock) on IIF(a.cpf is null, a.cnpj, a.cpf) = c.identifier
                      where b.doc_cliente is null and c.identifier is null and a.createddate > '2025-01-01'";
        }
    }
}
