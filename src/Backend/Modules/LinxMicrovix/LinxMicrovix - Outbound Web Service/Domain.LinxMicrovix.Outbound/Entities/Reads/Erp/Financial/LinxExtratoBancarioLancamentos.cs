using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Financial
{
    /// <summary>
    /// LinxExtratoBancarioLancamentos, tabela linxmicrovix.LinxExtratoBancarioLancamentos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + id_extrato_bancario_lancamentos
    /// </summary>
    public class LinxExtratoBancarioLancamentos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int id_extrato_bancario_lancamentos { get; private set; }
        public string? codigo_banco { get; private set; }
        public string? numero_conta { get; private set; }
        public long? id_conta { get; private set; }
        public decimal? valor { get; private set; }
        public string? descricao { get; private set; }
        public DateTime? data_lancamento { get; private set; }
        public long timestamp { get; private set; }

        public LinxExtratoBancarioLancamentos() { }

        public LinxExtratoBancarioLancamentos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Financial.LinxExtratoBancarioLancamentos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            id_extrato_bancario_lancamentos = MicrovixConverters.ToInt32(dto.id_extrato_bancario_lancamentos);
            codigo_banco = MicrovixConverters.ToNullableString(dto.codigo_banco, 4);
            numero_conta = MicrovixConverters.ToNullableString(dto.numero_conta, 20);
            id_conta = MicrovixConverters.ToNullableInt64(dto.id_conta);
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            descricao = MicrovixConverters.ToNullableString(dto.descricao, 100);
            data_lancamento = MicrovixConverters.ToNullableDateTime(dto.data_lancamento);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
