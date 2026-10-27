using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Payments
{
    /// <summary>
    /// LinxFechamentoCaixa, tabela linxmicrovix.LinxFechamentoCaixa. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + empresa + data + usuario (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxFechamentoCaixa
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int empresa { get; private set; }
        public DateTime data { get; private set; }
        public string usuario { get; private set; } = String.Empty;
        public int? qtd_001 { get; private set; }
        public int? qtd_005 { get; private set; }
        public int? qtd_010 { get; private set; }
        public int? qtd_025 { get; private set; }
        public int? qtd_050 { get; private set; }
        public int? qtd_1 { get; private set; }
        public int? qtd_2 { get; private set; }
        public int? qtd_5 { get; private set; }
        public int? qtd_10 { get; private set; }
        public int? qtd_20 { get; private set; }
        public int? qtd_50 { get; private set; }
        public int? qtd_100 { get; private set; }
        public int? qtd_200 { get; private set; }
        public decimal? total_geral { get; private set; }
        public string? obs { get; private set; }
        public decimal? total_c_vista { get; private set; }
        public decimal? total_c_prazo { get; private set; }
        public decimal? total_cartao { get; private set; }
        public decimal? total_convenio { get; private set; }
        public decimal? total_crediario { get; private set; }
        public decimal? total_giftcard { get; private set; }
        public decimal? total_qr_linx { get; private set; }
        public decimal? total_cartao_credito { get; private set; }
        public decimal? total_cartao_debito { get; private set; }
        public decimal? total_pix { get; private set; }
        public decimal? total_link_pagamento { get; private set; }
        public decimal? total_link_pagamento_credito { get; private set; }
        public decimal? total_link_pagamento_debito { get; private set; }
        public decimal? total_vale_compra { get; private set; }
        public decimal? vale_compras_dev { get; private set; }
        public decimal? sangria { get; private set; }
        public decimal? suprimentos { get; private set; }
        public bool? ativo { get; private set; }
        public bool? conferencia_efetuada { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxFechamentoCaixa() { }

        public LinxFechamentoCaixa(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Payments.LinxFechamentoCaixa dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            empresa = MicrovixConverters.ToInt32(dto.empresa);
            data = MicrovixConverters.ToNullableDateTime(dto.data) ?? new DateTime(1900, 1, 1);
            usuario = MicrovixConverters.ToNullableString(dto.usuario, 10) ?? String.Empty;
            qtd_001 = MicrovixConverters.ToNullableInt32(dto.qtd_001);
            qtd_005 = MicrovixConverters.ToNullableInt32(dto.qtd_005);
            qtd_010 = MicrovixConverters.ToNullableInt32(dto.qtd_010);
            qtd_025 = MicrovixConverters.ToNullableInt32(dto.qtd_025);
            qtd_050 = MicrovixConverters.ToNullableInt32(dto.qtd_050);
            qtd_1 = MicrovixConverters.ToNullableInt32(dto.qtd_1);
            qtd_2 = MicrovixConverters.ToNullableInt32(dto.qtd_2);
            qtd_5 = MicrovixConverters.ToNullableInt32(dto.qtd_5);
            qtd_10 = MicrovixConverters.ToNullableInt32(dto.qtd_10);
            qtd_20 = MicrovixConverters.ToNullableInt32(dto.qtd_20);
            qtd_50 = MicrovixConverters.ToNullableInt32(dto.qtd_50);
            qtd_100 = MicrovixConverters.ToNullableInt32(dto.qtd_100);
            qtd_200 = MicrovixConverters.ToNullableInt32(dto.qtd_200);
            total_geral = MicrovixConverters.ToNullableDecimal(dto.total_geral);
            obs = String.IsNullOrEmpty(dto.obs) ? null : dto.obs;
            total_c_vista = MicrovixConverters.ToNullableDecimal(dto.total_c_vista);
            total_c_prazo = MicrovixConverters.ToNullableDecimal(dto.total_c_prazo);
            total_cartao = MicrovixConverters.ToNullableDecimal(dto.total_cartao);
            total_convenio = MicrovixConverters.ToNullableDecimal(dto.total_convenio);
            total_crediario = MicrovixConverters.ToNullableDecimal(dto.total_crediario);
            total_giftcard = MicrovixConverters.ToNullableDecimal(dto.total_giftcard);
            total_qr_linx = MicrovixConverters.ToNullableDecimal(dto.total_qr_linx);
            total_cartao_credito = MicrovixConverters.ToNullableDecimal(dto.total_cartao_credito);
            total_cartao_debito = MicrovixConverters.ToNullableDecimal(dto.total_cartao_debito);
            total_pix = MicrovixConverters.ToNullableDecimal(dto.total_pix);
            total_link_pagamento = MicrovixConverters.ToNullableDecimal(dto.total_link_pagamento);
            total_link_pagamento_credito = MicrovixConverters.ToNullableDecimal(dto.total_link_pagamento_credito);
            total_link_pagamento_debito = MicrovixConverters.ToNullableDecimal(dto.total_link_pagamento_debito);
            total_vale_compra = MicrovixConverters.ToNullableDecimal(dto.total_vale_compra);
            vale_compras_dev = MicrovixConverters.ToNullableDecimal(dto.vale_compras_dev);
            sangria = MicrovixConverters.ToNullableDecimal(dto.sangria);
            suprimentos = MicrovixConverters.ToNullableDecimal(dto.suprimentos);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            conferencia_efetuada = MicrovixConverters.ToNullableBoolean(dto.conferencia_efetuada);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
