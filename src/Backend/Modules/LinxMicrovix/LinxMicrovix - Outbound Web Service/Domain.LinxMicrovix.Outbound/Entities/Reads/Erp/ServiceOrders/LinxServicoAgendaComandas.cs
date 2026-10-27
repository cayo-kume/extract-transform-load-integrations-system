using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// LinxServicoAgendaComandas, tabela linxmicrovix.LinxServicoAgendaComandas. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_comanda (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxServicoAgendaComandas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_comanda { get; private set; }
        public int? empresa { get; private set; }
        public int? cliente { get; private set; }
        public DateTime? data { get; private set; }
        public bool? noiva { get; private set; }
        public bool? excluida { get; private set; }
        public bool? fechada { get; private set; }
        public string? observacoes { get; private set; }
        public int? tipo_desconto { get; private set; }
        public decimal? valor_desconto { get; private set; }
        public long timestamp { get; private set; }

        public LinxServicoAgendaComandas() { }

        public LinxServicoAgendaComandas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaComandas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_comanda = MicrovixConverters.ToInt32(dto.id_comanda);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            cliente = MicrovixConverters.ToNullableInt32(dto.cliente);
            data = MicrovixConverters.ToNullableDateTime(dto.data);
            noiva = MicrovixConverters.ToNullableBoolean(dto.noiva);
            excluida = MicrovixConverters.ToNullableBoolean(dto.excluida);
            fechada = MicrovixConverters.ToNullableBoolean(dto.fechada);
            observacoes = MicrovixConverters.ToNullableString(dto.observacoes, 1000);
            tipo_desconto = MicrovixConverters.ToNullableInt32(dto.tipo_desconto);
            valor_desconto = MicrovixConverters.ToNullableDecimal(dto.valor_desconto);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
