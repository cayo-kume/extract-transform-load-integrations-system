using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.FiscalDocuments
{
    /// <summary>
    /// LinxReducoesZ, tabela linxmicrovix.LinxReducoesZ. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + numeroserie + numero_reducoes + aliquota (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxReducoesZ
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public string? cnpj { get; private set; }
        public DateTime? data_fechamento { get; private set; }
        public int? numero_ecf { get; private set; }
        public string numeroserie { get; private set; } = String.Empty;
        public int numero_reducoes { get; private set; }
        public int? cupom_inicial { get; private set; }
        public int? cupom_final { get; private set; }
        public decimal? gt_inicial { get; private set; }
        public decimal? gt_final { get; private set; }
        public int? qtde_cupons { get; private set; }
        public decimal? venda_bruta { get; private set; }
        public decimal? descontos { get; private set; }
        public decimal? acrescimos { get; private set; }
        public decimal? cancelamentos { get; private set; }
        public int? cro { get; private set; }
        public int? usuario { get; private set; }
        public string aliquota { get; private set; } = String.Empty;
        public decimal? valor { get; private set; }
        public DateTime? data_mov { get; private set; }
        public long timestamp { get; private set; }

        public LinxReducoesZ() { }

        public LinxReducoesZ(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.FiscalDocuments.LinxReducoesZ dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            cnpj = MicrovixConverters.ToNullableString(dto.cnpj, 14);
            data_fechamento = MicrovixConverters.ToNullableDateTime(dto.data_fechamento);
            numero_ecf = MicrovixConverters.ToNullableInt32(dto.numero_ecf);
            numeroserie = MicrovixConverters.ToNullableString(dto.numeroserie, 30) ?? String.Empty;
            numero_reducoes = MicrovixConverters.ToInt32(dto.numero_reducoes);
            cupom_inicial = MicrovixConverters.ToNullableInt32(dto.cupom_inicial);
            cupom_final = MicrovixConverters.ToNullableInt32(dto.cupom_final);
            gt_inicial = MicrovixConverters.ToNullableDecimal(dto.gt_inicial);
            gt_final = MicrovixConverters.ToNullableDecimal(dto.gt_final);
            qtde_cupons = MicrovixConverters.ToNullableInt32(dto.qtde_cupons);
            venda_bruta = MicrovixConverters.ToNullableDecimal(dto.venda_bruta);
            descontos = MicrovixConverters.ToNullableDecimal(dto.descontos);
            acrescimos = MicrovixConverters.ToNullableDecimal(dto.acrescimos);
            cancelamentos = MicrovixConverters.ToNullableDecimal(dto.cancelamentos);
            cro = MicrovixConverters.ToNullableInt32(dto.cro);
            usuario = MicrovixConverters.ToNullableInt32(dto.usuario);
            aliquota = MicrovixConverters.ToNullableString(dto.aliquota, 4) ?? String.Empty;
            valor = MicrovixConverters.ToNullableDecimal(dto.valor);
            data_mov = MicrovixConverters.ToNullableDateTime(dto.data_mov);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
