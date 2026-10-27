using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Returns
{
    /// <summary>
    /// LinxDevolucaoRemanejoFabrica, tabela linxmicrovix.LinxDevolucaoRemanejoFabrica. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_devolucao_remanejo_fabrica (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxDevolucaoRemanejoFabrica
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_devolucao_remanejo_fabrica { get; private set; }
        public int? id_devolucao_remanejo_fabrica_tipo { get; private set; }
        public int? id_motivo_devolucao_fabrica { get; private set; }
        public int? id_deposito { get; private set; }
        public int? id_devolucao_remanejo_fabrica_status { get; private set; }
        public int? empresa { get; private set; }
        public int? fornecedor { get; private set; }
        public string? cfop { get; private set; }
        public string? serie { get; private set; }
        public string? codigo_solicitacao { get; private set; }
        public int? portal { get; private set; }
        public DateTime? data_solicitacao { get; private set; }
        public long timestamp { get; private set; }
        public string? portal_2 { get; private set; }

        public LinxDevolucaoRemanejoFabrica() { }

        public LinxDevolucaoRemanejoFabrica(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Returns.LinxDevolucaoRemanejoFabrica dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_devolucao_remanejo_fabrica = MicrovixConverters.ToInt32(dto.id_devolucao_remanejo_fabrica);
            id_devolucao_remanejo_fabrica_tipo = MicrovixConverters.ToNullableInt32(dto.id_devolucao_remanejo_fabrica_tipo);
            id_motivo_devolucao_fabrica = MicrovixConverters.ToNullableInt32(dto.id_motivo_devolucao_fabrica);
            id_deposito = MicrovixConverters.ToNullableInt32(dto.id_deposito);
            id_devolucao_remanejo_fabrica_status = MicrovixConverters.ToNullableInt32(dto.id_devolucao_remanejo_fabrica_status);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            fornecedor = MicrovixConverters.ToNullableInt32(dto.fornecedor);
            cfop = MicrovixConverters.ToNullableString(dto.cfop, 10);
            serie = MicrovixConverters.ToNullableString(dto.serie, 10);
            codigo_solicitacao = MicrovixConverters.ToNullableString(dto.codigo_solicitacao, 50);
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            data_solicitacao = MicrovixConverters.ToNullableDateTime(dto.data_solicitacao);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal_2 = MicrovixConverters.ToNullableString(dto.portal_2, 1000);   //fora da documentação
        }
    }
}
