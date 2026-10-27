using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// LinxServicos, tabela linxmicrovix.LinxServicos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + cod_servico (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxServicos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public long cod_servico { get; private set; }
        public string? nome { get; private set; }
        public int? id_setor { get; private set; }
        public string? desc_setor { get; private set; }
        public int? id_linha { get; private set; }
        public string? desc_linha { get; private set; }
        public int? id_marca { get; private set; }
        public string? desc_marca { get; private set; }
        public DateTime? dt_update { get; private set; }
        public string? operacao_servico { get; private set; }
        public string? servico_km { get; private set; }
        public string? desativado { get; private set; }
        public string? cod_lc11603 { get; private set; }
        public string? codigo_nbs { get; private set; }
        public DateTime? dt_inclusao { get; private set; }
        public long timestamp { get; private set; }
        public string? codigo_ws { get; private set; }

        public LinxServicos() { }

        public LinxServicos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.ServiceOrders.LinxServicos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            cod_servico = MicrovixConverters.ToInt64(dto.cod_servico);
            nome = MicrovixConverters.ToNullableString(dto.nome, 1000);   //fora da documentação
            id_setor = MicrovixConverters.ToNullableInt32(dto.id_setor);
            desc_setor = MicrovixConverters.ToNullableString(dto.desc_setor, 30);
            id_linha = MicrovixConverters.ToNullableInt32(dto.id_linha);
            desc_linha = MicrovixConverters.ToNullableString(dto.desc_linha, 30);
            id_marca = MicrovixConverters.ToNullableInt32(dto.id_marca);
            desc_marca = MicrovixConverters.ToNullableString(dto.desc_marca, 30);
            dt_update = MicrovixConverters.ToNullableDateTime(dto.dt_update);
            operacao_servico = MicrovixConverters.ToNullableString(dto.operacao_servico, 1);
            servico_km = MicrovixConverters.ToNullableString(dto.servico_km, 1);
            desativado = MicrovixConverters.ToNullableString(dto.desativado, 1);
            cod_lc11603 = MicrovixConverters.ToNullableString(dto.cod_lc11603, 4);
            codigo_nbs = MicrovixConverters.ToNullableString(dto.codigo_nbs, 20);
            dt_inclusao = MicrovixConverters.ToNullableDateTime(dto.dt_inclusao);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            codigo_ws = MicrovixConverters.ToNullableString(dto.codigo_ws, 50);
        }
    }
}
