using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix
{
    public class LinxUsuarios
    {
        public DateTime lastupdateon { get; private set; }
        public Int32? usuario_id { get; private set; }
        public string? usuario_login { get; private set; }
        public string? usuario_nome { get; private set; }
        public string? usuario_email { get; private set; }
        public Int32? usuario_grupo_id { get; private set; }
        public string? grupo_usuarios { get; private set; }
        public string? usuario_supervisor { get; private set; }
        public string? usuario_doc { get; private set; }
        public Int32? vendedor { get; private set; }
        public DateTime? data_criacao { get; private set; }
        public string? desativado { get; private set; }
        public string? empresas { get; private set; }
        public Int32? portal { get; private set; }
        public Int64? timestamp { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordKey { get; private set; }

        [NotMapped]
        [SkipProperty]
        public string? recordXml { get; private set; }

        public LinxUsuarios() { }

        public LinxUsuarios(Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix.LinxUsuarios record, string recordXml)
        {
            lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.usuario_id = ConverterValidatorsExtensions.ConvertToInt32Validation(record.usuario_id);
            this.usuario_grupo_id = ConverterValidatorsExtensions.ConvertToInt32Validation(record.usuario_grupo_id);
            this.vendedor = ConverterValidatorsExtensions.ConvertToInt32Validation(record.vendedor);
            this.data_criacao = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(record.data_criacao);
            this.portal = ConverterValidatorsExtensions.ConvertToInt32Validation(record.portal);
            this.timestamp = ConverterValidatorsExtensions.ConvertToInt64Validation(record.timestamp);
            this.usuario_login = record.usuario_login;
            this.usuario_nome = record.usuario_nome;
            this.usuario_email = record.usuario_email;
            this.grupo_usuarios = record.grupo_usuarios;
            this.usuario_supervisor = record.usuario_supervisor;
            this.usuario_doc = record.usuario_doc;
            this.desativado = record.desativado;
            this.empresas = record.empresas;
            this.recordKey = $"[{record.usuario_id}]|[{record.usuario_doc}]|[{record.timestamp}]";
            this.recordXml = recordXml;
        }
    }
}
