using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxLancContabil, tabela linxmicrovix.LinxLancContabil. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_lanc (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxLancContabil
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long? cod_lanc { get; private set; }
        public string? centro_custo { get; private set; }
        public string? ind_conta { get; private set; }
        public long? cod_conta { get; private set; }
        public string? nome_conta { get; private set; }
        public decimal? valor_conta { get; private set; }
        public string? cred_deb { get; private set; }
        public DateTime? data_lanc { get; private set; }
        public string? compl_conta { get; private set; }
        public Guid? identificador { get; private set; }
        public long? cod_historico { get; private set; }
        public string? desc_historico { get; private set; }
        public DateTime? data_compensacao { get; private set; }
        public int? fatura_origem { get; private set; }
        public string? efetivado { get; private set; }
        public long timestamp { get; private set; }
        public int? empresa { get; private set; }
        public long id_lanc { get; private set; }
        public string? cancelado { get; private set; }

        public LinxLancContabil() { }

        public LinxLancContabil(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxLancContabil dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_lanc = MicrovixConverters.ToNullableInt64(dto.cod_lanc);
            centro_custo = MicrovixConverters.ToNullableString(dto.centro_custo, 50);
            ind_conta = MicrovixConverters.ToNullableString(dto.ind_conta, 150);
            cod_conta = MicrovixConverters.ToNullableInt64(dto.cod_conta);
            nome_conta = MicrovixConverters.ToNullableString(dto.nome_conta, 50);
            valor_conta = MicrovixConverters.ToNullableDecimal(dto.valor_conta);
            cred_deb = MicrovixConverters.ToNullableString(dto.cred_deb, 1);
            data_lanc = MicrovixConverters.ToNullableDateTime(dto.data_lanc);
            compl_conta = MicrovixConverters.ToNullableString(dto.compl_conta, 500);
            identificador = MicrovixConverters.ToNullableGuid(dto.identificador);
            cod_historico = MicrovixConverters.ToNullableInt64(dto.cod_historico);
            desc_historico = MicrovixConverters.ToNullableString(dto.desc_historico, 50);
            data_compensacao = MicrovixConverters.ToNullableDateTime(dto.data_compensacao);
            fatura_origem = MicrovixConverters.ToNullableInt32(dto.fatura_origem);
            efetivado = MicrovixConverters.ToNullableString(dto.efetivado, 1);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_lanc = MicrovixConverters.ToInt64(dto.id_lanc);
            cancelado = MicrovixConverters.ToNullableString(dto.cancelado, 1);
        }
    }
}
