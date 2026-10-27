using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Stores
{
    /// <summary>
    /// B2CConsultaVendedores, tabela linxmicrovix.B2CConsultaVendedores. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_vendedor
    /// </summary>
    public class B2CConsultaVendedores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int cod_vendedor { get; private set; }
        public string? nome_vendedor { get; private set; }
        public decimal? comissao_produtos { get; private set; }
        public decimal? comissao_servicos { get; private set; }
        public string? tipo { get; private set; }
        public bool? ativo { get; private set; }
        public int? comissionado { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaVendedores() { }

        public B2CConsultaVendedores(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Stores.B2CConsultaVendedores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            cod_vendedor = MicrovixConverters.ToInt32(dto.cod_vendedor);
            nome_vendedor = MicrovixConverters.ToNullableString(dto.nome_vendedor, 50);
            comissao_produtos = MicrovixConverters.ToNullableDecimal(dto.comissao_produtos);
            comissao_servicos = MicrovixConverters.ToNullableDecimal(dto.comissao_servicos);
            tipo = MicrovixConverters.ToNullableString(dto.tipo, 1);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            comissionado = MicrovixConverters.ToNullableInt32(dto.comissionado);   //fora da documentação
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
