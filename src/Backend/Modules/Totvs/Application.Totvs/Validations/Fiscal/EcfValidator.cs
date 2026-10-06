using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class EcfValidator : AbstractValidator<Ecf>
    {
        public EcfValidator()
        {
            RuleFor(x => x.coupon)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.coupon));

            RuleFor(x => x.serialMachine)
                .MaximumLength(60)
                .WithMessage("O campo 'serialMachine' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serialMachine));
        }
    }
}
