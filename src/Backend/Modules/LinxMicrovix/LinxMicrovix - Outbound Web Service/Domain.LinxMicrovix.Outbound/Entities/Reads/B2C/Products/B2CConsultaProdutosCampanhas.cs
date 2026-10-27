using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Products
{
    /// <summary>
    /// B2CConsultaProdutosCampanhas, tabela linxmicrovix.B2CConsultaProdutosCampanhas. Método de loja (um por CNPJ de B2CConsultaEmpresas): chave cnpj_emp + codigo_campanha (a confirmar com uma resposta real com dados)
    /// </summary>
    public class B2CConsultaProdutosCampanhas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int codigo_campanha { get; private set; }
        public string? nome_campanha { get; private set; }
        public DateTime? vigencia_inicio { get; private set; }
        public DateTime? vigencia_fim { get; private set; }
        public string? observacao { get; private set; }
        public bool? ativo { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public B2CConsultaProdutosCampanhas() { }

        public B2CConsultaProdutosCampanhas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Products.B2CConsultaProdutosCampanhas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            codigo_campanha = MicrovixConverters.ToInt32(dto.codigo_campanha);
            nome_campanha = MicrovixConverters.ToNullableString(dto.nome_campanha, 60);
            vigencia_inicio = MicrovixConverters.ToNullableDateTime(dto.vigencia_inicio);
            vigencia_fim = MicrovixConverters.ToNullableDateTime(dto.vigencia_fim);
            observacao = String.IsNullOrEmpty(dto.observacao) ? null : dto.observacao;
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
