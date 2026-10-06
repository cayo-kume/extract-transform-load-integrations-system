
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxProdutosTabelas
    {
        public DateTime? lastupdateon { get; private set; }
        public Int32? id_tabela { get; private set; }
        public Int32? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? nome_tabela { get; private set; }
        public string? ativa { get; private set; }
        public Int64? timestamp { get; private set; }
        public string? tipo_tabela { get; private set; }
        public string? codigo_integracao_ws { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxProdutosTabelas() { }

        public LinxProdutosTabelas(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxProdutosTabelas record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id_tabela = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_tabela);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.cnpj_emp = record.cnpj_emp;
            this.nome_tabela = record.nome_tabela;
            this.tipo_tabela = record.tipo_tabela;
            this.ativa = record.ativa;
            this.codigo_integracao_ws = record.codigo_integracao_ws;
            this.recordKey = $"[{record.cnpj_emp}]|[{record.id_tabela}]|[{record.tipo_tabela}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
