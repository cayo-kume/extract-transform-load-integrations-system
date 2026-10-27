using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stocks
{
    /// <summary>
    /// LinxListagemBalanco, tabela linxmicrovix.LinxListagemBalanco. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_balanco
    /// </summary>
    public class LinxListagemBalanco
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_balanco { get; private set; }
        public DateTime? data { get; private set; }
        public string? nome_arquivo { get; private set; }
        public string? processado { get; private set; }
        public DateTime? data_ultimo_processamento { get; private set; }
        public string? qtde_produtos { get; private set; }
        public int? qtde_itens { get; private set; }
        public long timestamp { get; private set; }
        public int? portal { get; private set; }

        public LinxListagemBalanco() { }

        public LinxListagemBalanco(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stocks.LinxListagemBalanco dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_balanco = MicrovixConverters.ToInt32(dto.id_balanco);
            data = MicrovixConverters.ToNullableDateTime(dto.data);
            nome_arquivo = MicrovixConverters.ToNullableString(dto.nome_arquivo, 50);
            processado = MicrovixConverters.ToNullableString(dto.processado, 1);
            data_ultimo_processamento = MicrovixConverters.ToNullableDateTime(dto.data_ultimo_processamento);
            qtde_produtos = MicrovixConverters.ToNullableString(dto.qtde_produtos, 1000);   //int na documentação; a resposta real não cabe nesse tipo
            qtde_itens = MicrovixConverters.ToNullableInt32(dto.qtde_itens);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
        }
    }
}
