using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Movements
{
    /// <summary>
    /// LinxMovimentoPrincipal, tabela linxmicrovix.LinxMovimentoPrincipal. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_movimento_principal
    /// </summary>
    public class LinxMovimentoPrincipal
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_movimento_principal { get; private set; }
        public Guid? identificador { get; private set; }
        public long? codigoproduto_manutencao { get; private set; }
        public long timestamp { get; private set; }
        public int? id_pergunta_venda { get; private set; }
        public int? id_resposta_venda { get; private set; }
        public decimal? total_fidelidade_cashback { get; private set; }
        public int? plano_fidelidade_cashback { get; private set; }
        public string? remessa_pedido_compra { get; private set; }
        public int? id_motivo_desconto { get; private set; }
        public decimal? valor_nota { get; private set; }
        public int? portal { get; private set; }
        public bool? estornada { get; private set; }
        public string? motivo_desconto_observacao { get; private set; }
        public bool? nota_aguardou_sefaz { get; private set; }

        public LinxMovimentoPrincipal() { }

        public LinxMovimentoPrincipal(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Movements.LinxMovimentoPrincipal dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_movimento_principal = MicrovixConverters.ToInt32(dto.id_movimento_principal);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            codigoproduto_manutencao = MicrovixConverters.ToNullableInt64(dto.codigoproduto_manutencao);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            id_pergunta_venda = MicrovixConverters.ToNullableInt32(dto.id_pergunta_venda);
            id_resposta_venda = MicrovixConverters.ToNullableInt32(dto.id_resposta_venda);
            total_fidelidade_cashback = MicrovixConverters.ToNullableDecimal(dto.total_fidelidade_cashback);
            plano_fidelidade_cashback = MicrovixConverters.ToNullableInt32(dto.plano_fidelidade_cashback);
            remessa_pedido_compra = MicrovixConverters.ToNullableString(dto.remessa_pedido_compra, 30);
            id_motivo_desconto = MicrovixConverters.ToNullableInt32(dto.id_motivo_desconto);
            valor_nota = MicrovixConverters.ToNullableDecimal(dto.valor_nota);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            estornada = MicrovixConverters.ToNullableBoolean(dto.estornada);
            motivo_desconto_observacao = MicrovixConverters.ToNullableString(dto.motivo_desconto_observacao, 500);
            nota_aguardou_sefaz = MicrovixConverters.ToNullableBoolean(dto.nota_aguardou_sefaz);
        }
    }
}
