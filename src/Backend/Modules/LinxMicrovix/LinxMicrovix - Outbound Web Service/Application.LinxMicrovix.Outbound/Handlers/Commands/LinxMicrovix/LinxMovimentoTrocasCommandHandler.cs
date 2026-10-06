using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxMovimentoTrocasCommandHandler : ILinxMovimentoTrocasCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<LinxMovimentoTrocas> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r.num_vale}'"));
            return $"SELECT CONCAT('[', CNPJ_EMP, ']', '|', '[', NUM_VALE, ']', '|', '[', DOC_ORIGEM, ']', '|', '[', DOC_VENDA, ']', '|', '[', DOC_VENDA_ORIGEM, ']', '|', '[', COD_CLIENTE, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxMovimentoTrocas] WHERE num_vale IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                            ([lastupdateon],[portal],[cnpj_emp],[identificador],[num_vale],[valor_vale],[motivo],[doc_origem],[serie_origem],[doc_venda],[serie_venda],[excluido]
                            ,[timestamp],[desfazimento],[empresa],[vale_cod_cliente],[vale_codigoproduto],[id_vale_ordem_servico_externa],[doc_venda_origem],[serie_venda_origem],[cod_cliente])
                            Values
                            (@lastupdateon,@portal,@cnpj_emp,@identificador,@num_vale,@valor_vale,@motivo,@doc_origem,@serie_origem,@doc_venda,@serie_venda,@excluido,@timestamp,@desfazimento,
                             @empresa,@vale_cod_cliente,@vale_codigoproduto,@id_vale_ordem_servico_externa,@doc_venda_origem,@serie_venda_origem,@cod_cliente)";
        }
    }
}
