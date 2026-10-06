using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxComissoesVendedoresValidator : AbstractValidator<LinxComissoesVendedores>
    {
        public LinxComissoesVendedoresValidator()
        {
            RuleFor(x => x.vendedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.vendedor));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.data_origem).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_origem));
            RuleFor(x => x.valor_base).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_base));
            RuleFor(x => x.valor_comissao).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_comissao));
            RuleFor(x => x.cancelado).MaximumLength(1).WithMessage("O campo 'cancelado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.cancelado));
            RuleFor(x => x.disponivel).MaximumLength(1).WithMessage("O campo 'disponivel' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.disponivel));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
