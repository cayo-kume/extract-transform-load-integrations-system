using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxPosicaoOsRamoOpticoValidator : AbstractValidator<LinxPosicaoOsRamoOptico>
    {
        public LinxPosicaoOsRamoOpticoValidator()
        {
            RuleFor(x => x.id_posicao_os_ramo_optico).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_posicao_os_ramo_optico));
            RuleFor(x => x.descricao).MaximumLength(50).WithMessage("O campo 'descricao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.codigo_status_padrao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_status_padrao));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
