using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxHistoricosContabeis, tabela linxmicrovix.LinxHistoricosContabeis. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_historico
    /// </summary>
    public class LinxHistoricosContabeis
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public long id_historico { get; private set; }
        public string? nome_historico { get; private set; }
        public long? conta_credito { get; private set; }
        public long? conta_debito { get; private set; }
        public string? tipo_rateio { get; private set; }
        public string? inativo { get; private set; }
        public string? operacao { get; private set; }
        public int? historico_contabil { get; private set; }
        public bool? somente_gerencial { get; private set; }
        public bool? obrigatorio_insercao_anexo_sangria { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public LinxHistoricosContabeis() { }

        public LinxHistoricosContabeis(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxHistoricosContabeis dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id_historico = MicrovixConverters.ToInt64(dto.id_historico);
            nome_historico = MicrovixConverters.ToNullableString(dto.nome_historico, 50);
            conta_credito = MicrovixConverters.ToNullableInt64(dto.conta_credito);
            conta_debito = MicrovixConverters.ToNullableInt64(dto.conta_debito);
            tipo_rateio = MicrovixConverters.ToNullableString(dto.tipo_rateio, 1);
            inativo = MicrovixConverters.ToNullableString(dto.inativo, 1);
            operacao = MicrovixConverters.ToNullableString(dto.operacao, 1);
            historico_contabil = MicrovixConverters.ToNullableInt32(dto.historico_contabil);
            somente_gerencial = MicrovixConverters.ToNullableBoolean(dto.somente_gerencial);
            obrigatorio_insercao_anexo_sangria = MicrovixConverters.ToNullableBoolean(dto.obrigatorio_insercao_anexo_sangria);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
