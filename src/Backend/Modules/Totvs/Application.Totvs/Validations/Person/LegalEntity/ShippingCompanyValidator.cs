using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Person.LegalEntity;
using FluentValidation;

namespace Application.Totvs.Validations.Person.LegalEntity
{
    public class ShippingCompanyValidator : AbstractValidator<Shippingcompany>
    {
        public ShippingCompanyValidator()
        {
            RuleFor(x => x.percentegeValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentegeValue));

            RuleFor(x => x.kilogramsValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.kilogramsValue));

            RuleFor(x => x.minimunValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.minimunValue));

            RuleFor(x => x.maximumValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.maximumValue));

            RuleFor(x => x.anttCode)
                .MaximumLength(60)
                .WithMessage("O campo 'anttCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.anttCode));


            RuleFor(x => x.typeDeductIcms)
                .MaximumLength(60)
                .WithMessage("O campo 'typeDeductIcms' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeDeductIcms));
        }
    }
}
