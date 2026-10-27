using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Consignments
{
    /// <summary>
    /// LinxMovimentoRemessasAcertos, tabela linxmicrovix.LinxMovimentoRemessasAcertos. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_remessas_acertos
    /// </summary>
    public class LinxMovimentoRemessasAcertos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int? portal { get; private set; }
        public int? empresa { get; private set; }
        public int id_remessas_acertos { get; private set; }
        public int? id_remessas { get; private set; }
        public Guid? identificador_venda { get; private set; }
        public Guid? identificador_retorno { get; private set; }
        public Guid? identificador_devolucao { get; private set; }
        public DateTime? data { get; private set; }
        public long? id_vendas_pos { get; private set; }
        public bool? excluido { get; private set; }
        public long timestamp { get; private set; }

        public LinxMovimentoRemessasAcertos() { }

        public LinxMovimentoRemessasAcertos(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Consignments.LinxMovimentoRemessasAcertos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            empresa = MicrovixConverters.ToNullableInt32(dto.empresa);
            id_remessas_acertos = MicrovixConverters.ToInt32(dto.id_remessas_acertos);
            id_remessas = MicrovixConverters.ToNullableInt32(dto.id_remessas);
            identificador_venda = MicrovixConverters.ToNullableGuid(dto.identificador_venda);
            identificador_retorno = MicrovixConverters.ToNullableGuid(dto.identificador_retorno);
            identificador_devolucao = MicrovixConverters.ToNullableGuid(dto.identificador_devolucao);
            data = MicrovixConverters.ToNullableDateTime(dto.data);
            id_vendas_pos = MicrovixConverters.ToNullableInt64(dto.id_vendas_pos);
            excluido = MicrovixConverters.ToNullableBoolean(dto.excluido);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
