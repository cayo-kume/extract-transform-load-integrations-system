using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxCommerce;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxCommerce
{
    public class B2CConsultaNFeCommandHandler : IB2CConsultaNFeCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', ID_NFE, ']', '|', '[', ID_PEDIDO, ']', '|', '[', CHAVE_NFE, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_commerce].[B2CCONSULTANFE] WHERE ID_NFE IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                          ([lastupdateon], [id_nfe], [id_pedido], [documento], [data_emissao], [chave_nfe], [situacao], [xml], [excluido], [identificador_microvix], [dt_insert], [valor_nota], [serie], [frete], [timestamp], [portal], [nProt], [codigo_modelo_nf], [justificativa], [tpAmb]) 
                          Values 
                          (@lastupdateon, @id_nfe, @id_pedido, @documento, @data_emissao, @chave_nfe, @situacao, @xml, @excluido, @identificador_microvix, @dt_insert, @valor_nota, @serie, @frete, @timestamp, @portal, @nProt, @codigo_modelo_nf, @justificativa, @tpAmb)";
        }

        public string CreateIntegrityLockQuery()
        {
            return @$"select distinct top 500 
                      'B2CConsultaNFe' as [table], 
                      'id_pedido' as recordKey, 
                      a.id_pedido as identifier  
                      from linx_microvix_commerce.b2cconsultapedidos a (nolock)
                      join linx_microvix_commerce.b2cconsultapedidosstatus b (nolock) on a.id_pedido = b.id_pedido
                      left join linx_microvix_commerce.b2cconsultanfe c (nolock) on a.id_pedido = c.id_pedido
                      where c.id_pedido is null and b.id_status = 2 and a.dt_pedido > '2025-01-01'";
        }
    }
}
