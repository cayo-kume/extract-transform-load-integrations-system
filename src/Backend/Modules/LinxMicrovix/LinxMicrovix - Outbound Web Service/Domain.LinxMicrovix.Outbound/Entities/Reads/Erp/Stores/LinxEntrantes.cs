using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores
{
    /// <summary>
    /// contagem de entrantes por período e loja (LinxEntrantes), tabela linxmicrovix.LinxEntrantes. Método de portal (sem
    /// cnpjEmp), que só devolve os últimos 2 anos (v274): chave portal + id_entrantes
    /// </summary>
    public class LinxEntrantes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public long id_entrantes { get; private set; }
        public DateTime? data_inicio { get; private set; }
        public DateTime? data_fim { get; private set; }
        public string? cnpj_empresa { get; private set; }
        public int? entrantes { get; private set; }
        public long timestamp { get; private set; }

        public LinxEntrantes() { }

        public LinxEntrantes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stores.LinxEntrantes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_entrantes = MicrovixConverters.ToInt64(dto.id_entrantes);
            data_inicio = MicrovixConverters.ToNullableDateTime(dto.data_inicio);
            data_fim = MicrovixConverters.ToNullableDateTime(dto.data_fim);
            cnpj_empresa = MicrovixConverters.ToNullableString(dto.cnpj_empresa, 14);
            entrantes = MicrovixConverters.ToNullableInt32(dto.entrantes);   //fora da v274: tipo a conferir quando vier preenchido
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
