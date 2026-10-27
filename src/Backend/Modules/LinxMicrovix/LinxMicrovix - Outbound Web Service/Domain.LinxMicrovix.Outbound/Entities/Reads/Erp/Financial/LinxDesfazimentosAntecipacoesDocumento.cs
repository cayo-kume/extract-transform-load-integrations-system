using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxDesfazimentosAntecipacoesDocumento, tabela linxmicrovix.LinxDesfazimentosAntecipacoesDocumento. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_documento_desfazimento_optico (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxDesfazimentosAntecipacoesDocumento
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_documento_desfazimento_optico { get; private set; }
        public int? empresa { get; private set; }
        public string? id_desfazimentos_antecipacoes { get; private set; }
        public int? id_antecipacoes_financeiras { get; private set; }
        public string? cfop { get; private set; }
        public int? cod_cliente { get; private set; }
        public decimal? valor_operacao_nao_desfeita { get; private set; }
        public string? texto_descricao_desfazimento { get; private set; }
        public long timestamp { get; private set; }

        public LinxDesfazimentosAntecipacoesDocumento() { }

        public LinxDesfazimentosAntecipacoesDocumento(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxDesfazimentosAntecipacoesDocumento dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_documento_desfazimento_optico = MicrovixConverters.ToInt32(dto.id_documento_desfazimento_optico);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_desfazimentos_antecipacoes = MicrovixConverters.ToNullableString(dto.id_desfazimentos_antecipacoes, 1000);   //fora da documentação
            id_antecipacoes_financeiras = MicrovixConverters.ToNullableInt32(dto.id_antecipacoes_financeiras);
            cfop = MicrovixConverters.ToNullableString(dto.cfop, 10);
            cod_cliente = MicrovixConverters.ToNullableInt32(dto.cod_cliente);
            valor_operacao_nao_desfeita = MicrovixConverters.ToNullableDecimal(dto.valor_operacao_nao_desfeita);
            texto_descricao_desfazimento = String.IsNullOrEmpty(dto.texto_descricao_desfazimento) ? null : dto.texto_descricao_desfazimento;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
