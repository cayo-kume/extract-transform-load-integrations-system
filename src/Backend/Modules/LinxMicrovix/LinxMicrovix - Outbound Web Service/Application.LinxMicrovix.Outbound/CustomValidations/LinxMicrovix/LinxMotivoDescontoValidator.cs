using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMotivoDescontoValidator : AbstractValidator<LinxMotivoDesconto>
    {
        public LinxMotivoDescontoValidator()
        {
            RuleFor(x => x.id_motivo_desconto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_motivo_desconto));
            RuleFor(x => x.desc_motivo_desconto).MaximumLength(60).WithMessage("O campo 'desc_motivo_desconto' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_motivo_desconto));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.excluido).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.excluido));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}