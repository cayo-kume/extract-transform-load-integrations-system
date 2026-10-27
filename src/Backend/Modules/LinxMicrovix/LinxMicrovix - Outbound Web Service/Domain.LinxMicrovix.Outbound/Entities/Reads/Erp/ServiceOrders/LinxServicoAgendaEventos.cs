using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// LinxServicoAgendaEventos, tabela linxmicrovix.LinxServicoAgendaEventos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_evento (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxServicoAgendaEventos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int id_evento { get; private set; }
        public int? id_espaco { get; private set; }
        public DateTime? data_evento { get; private set; }
        public int? minutos_duracao { get; private set; }
        public int? tipo_agendamento { get; private set; }
        public int? id_cliente_servico { get; private set; }
        public int? id_profissional { get; private set; }
        public int? situacao { get; private set; }
        public DateTime? data_situacao { get; private set; }
        public string? observacao { get; private set; }
        public DateTime? data_chegada { get; private set; }
        public DateTime? data_execucao { get; private set; }
        public bool? recorrente { get; private set; }
        public int? id_comanda { get; private set; }
        public long timestamp { get; private set; }

        public LinxServicoAgendaEventos() { }

        public LinxServicoAgendaEventos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ServiceOrders.LinxServicoAgendaEventos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            id_evento = MicrovixConverters.ToInt32(dto.id_evento);
            id_espaco = MicrovixConverters.ToNullableInt32(dto.id_espaco);
            data_evento = MicrovixConverters.ToNullableDateTime(dto.data_evento);
            minutos_duracao = MicrovixConverters.ToNullableInt32(dto.minutos_duracao);
            tipo_agendamento = MicrovixConverters.ToNullableInt32(dto.tipo_agendamento);
            id_cliente_servico = MicrovixConverters.ToNullableInt32(dto.id_cliente_servico);
            id_profissional = MicrovixConverters.ToNullableInt32(dto.id_profissional);
            situacao = MicrovixConverters.ToNullableInt32(dto.situacao);
            data_situacao = MicrovixConverters.ToNullableDateTime(dto.data_situacao);
            observacao = MicrovixConverters.ToNullableString(dto.observacao, 1000);
            data_chegada = MicrovixConverters.ToNullableDateTime(dto.data_chegada);
            data_execucao = MicrovixConverters.ToNullableDateTime(dto.data_execucao);
            recorrente = MicrovixConverters.ToNullableBoolean(dto.recorrente);
            id_comanda = MicrovixConverters.ToNullableInt32(dto.id_comanda);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
