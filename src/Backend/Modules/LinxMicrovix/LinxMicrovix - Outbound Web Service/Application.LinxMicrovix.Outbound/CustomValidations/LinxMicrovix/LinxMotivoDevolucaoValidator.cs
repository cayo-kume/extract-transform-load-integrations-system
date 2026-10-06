using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMotivoDevolucaoValidator : AbstractValidator<LinxMotivoDevolucao>
    {
        public LinxMotivoDevolucaoValidator()
        {
            RuleFor(x => x.cod_motivo).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_motivo));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.descricao_motivo).MaximumLength(50).WithMessage("O campo 'descricao_motivo' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_motivo));
            RuleFor(x => x.cod_deposito).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_deposito));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}