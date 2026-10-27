using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.TaxSettings;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.TaxSettings
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script016_Create_LinxMicrovix_Tax_Settings_Tables
    /// </summary>
    public class LinxConfiguracoesTributariasEmpresasValidator : AbstractValidator<LinxConfiguracoesTributariasEmpresas>
    {
        public LinxConfiguracoesTributariasEmpresasValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_config_tributaria)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_config_tributaria));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
