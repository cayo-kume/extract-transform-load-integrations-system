using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// LinxServicoClientes, tabela linxmicrovix.LinxServicoClientes. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_cliente + id_servico + id_dav_item (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxServicoClientes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_cliente { get; private set; }
        public int? empresa { get; private set; }
        public long id_servico { get; private set; }
        public int? qtd_adquirido { get; private set; }
        public int? qtd_agendado { get; private set; }
        public int? qtd_utilizado { get; private set; }
        public int? qtd_cancelado { get; private set; }
        public DateTime? dt_insercao { get; private set; }
        public int? tipo_desconto { get; private set; }
        public decimal? valor_desconto_por_item { get; private set; }
        public long id_dav_item { get; private set; }
        public int? transacao_movimento { get; private set; }
        public int? ficha { get; private set; }
        public long timestamp { get; private set; }
        public string? identificador { get; private set; }

        public LinxServicoClientes() { }

        public LinxServicoClientes(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ServiceOrders.LinxServicoClientes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_cliente = MicrovixConverters.ToInt32(dto.id_cliente);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_servico = MicrovixConverters.ToInt64(dto.id_servico);
            qtd_adquirido = MicrovixConverters.ToNullableInt32(dto.qtd_adquirido);
            qtd_agendado = MicrovixConverters.ToNullableInt32(dto.qtd_agendado);
            qtd_utilizado = MicrovixConverters.ToNullableInt32(dto.qtd_utilizado);
            qtd_cancelado = MicrovixConverters.ToNullableInt32(dto.qtd_cancelado);
            dt_insercao = MicrovixConverters.ToNullableDateTime(dto.dt_insercao);
            tipo_desconto = MicrovixConverters.ToNullableInt32(dto.tipo_desconto);
            valor_desconto_por_item = MicrovixConverters.ToNullableDecimal(dto.valor_desconto_por_item);
            id_dav_item = MicrovixConverters.ToInt64(dto.id_dav_item);
            transacao_movimento = MicrovixConverters.ToNullableInt32(dto.transacao_movimento);
            ficha = MicrovixConverters.ToNullableInt32(dto.ficha);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            identificador = MicrovixConverters.ToNullableString(dto.identificador, 1000);   //fora da documentação
        }
    }
}
