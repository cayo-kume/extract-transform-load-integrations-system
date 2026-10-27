using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxDesfazimentosAntecipacoesItens, tabela linxmicrovix.LinxDesfazimentosAntecipacoesItens. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_desfazimentos_antecipacoes_itens (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxDesfazimentosAntecipacoesItens
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_desfazimentos_antecipacoes_itens { get; private set; }
        public string? id_desfazimentos_antecipacoes { get; private set; }
        public string? numero_sequencial { get; private set; }
        public int? ordem { get; private set; }
        public Guid? identificador { get; private set; }
        public decimal? preco { get; private set; }
        public decimal? quantidade { get; private set; }
        public decimal? desconto { get; private set; }
        public decimal? valor { get; private set; }
        public long timestamp { get; private set; }

        public LinxDesfazimentosAntecipacoesItens() { }

        public LinxDesfazimentosAntecipacoesItens(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxDesfazimentosAntecipacoesItens dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_desfazimentos_antecipacoes_itens = MicrovixConverters.ToInt32(dto.id_desfazimentos_antecipacoes_itens);
            id_desfazimentos_antecipacoes = MicrovixConverters.ToNullableString(dto.id_desfazimentos_antecipacoes, 1000);   //fora da documentação
            numero_sequencial = MicrovixConverters.ToNullableString(dto.numero_sequencial, 1000);   //fora da documentação
            ordem = MicrovixConverters.ToNullableInt32(dto.ordem);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            preco = MicrovixConverters.ToNullableDecimal(dto.preco);
            quantidade = MicrovixConverters.ToNullableDecimal(dto.quantidade);
            desconto = MicrovixConverters.ToNullableDecimal(dto.desconto);
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
