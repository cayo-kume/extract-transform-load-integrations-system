
using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxGrupoLojas
    {
        public DateTime? lastupdateon { get; private set; }
        public string? cnpj { get; private set; }
        public string? nome_empresa { get; private set; }
        public Int32? id_empresas_rede { get; private set; }
        public string? rede { get; private set; }
        public Int32? portal { get; private set; }
        public string? nome_portal { get; private set; }
        public Int32? empresa { get; private set; }
        public string? classificacao_portal { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxGrupoLojas() { }

        public LinxGrupoLojas(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxGrupoLojas record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.id_empresas_rede = ConverterValidatorsExtensions.ConvertToInt32Validation(record.id_empresas_rede);
            this.empresa = ConverterValidatorsExtensions.ConvertToInt32Validation(record.empresa);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.cnpj = record.cnpj;
            this.rede = record.rede;
            this.nome_empresa = record.nome_empresa;
            this.nome_portal = record.nome_portal;
            this.classificacao_portal = record.classificacao_portal;
            this.recordKey = $"[{record.cnpj}]|[{record.nome_empresa}]|[{record.id_empresas_rede}]|[{record.rede}]|[{record.portal}]|[{record.nome_portal}]|[{record.empresa}]|[{record.classificacao_portal}]";
            this.recordXml = recordXml;
        }
    }
}
