using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Stores
{
    /// <summary>
    /// B2CConsultaPlanos, tabela linxmicrovix.B2CConsultaPlanos. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + plano
    /// </summary>
    public class B2CConsultaPlanos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int plano { get; private set; }
        public string? nome_plano { get; private set; }
        public int? forma_pagamento { get; private set; }
        public int? qtde_parcelas { get; private set; }
        public decimal? valor_minimo_parcela { get; private set; }
        public decimal? indice { get; private set; }
        public long timestamp { get; private set; }
        public string? desativado { get; private set; }
        public string? tipo_plano { get; private set; }
        public int portal { get; private set; }
        public string? usa_tef { get; private set; }

        public B2CConsultaPlanos() { }

        public B2CConsultaPlanos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Stores.B2CConsultaPlanos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            plano = MicrovixConverters.ToInt32(dto.plano);
            nome_plano = MicrovixConverters.ToNullableString(dto.nome_plano, 100);
            forma_pagamento = MicrovixConverters.ToNullableInt32(dto.forma_pagamento);
            qtde_parcelas = MicrovixConverters.ToNullableInt32(dto.qtde_parcelas);
            valor_minimo_parcela = MicrovixConverters.ToNullableDecimal(dto.valor_minimo_parcela);
            indice = MicrovixConverters.ToNullableDecimal(dto.indice);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            desativado = MicrovixConverters.ToNullableString(dto.desativado, 1);
            tipo_plano = MicrovixConverters.ToNullableString(dto.tipo_plano, 1);
            portal = MicrovixConverters.ToInt32(dto.portal);
            usa_tef = MicrovixConverters.ToNullableString(dto.usa_tef, 1);
        }
    }
}
