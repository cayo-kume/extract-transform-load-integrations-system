using Domain.LinxCommerce.Entities.SalesRepresentative;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.SalesRepresentative
{
    public class SalesRepresentativeMaxDiscountValidator : AbstractValidator<SalesRepresentativeMaxDiscount>
    {
        public SalesRepresentativeMaxDiscountValidator()
        {
            RuleFor(x => x.Type)
                .MaximumLength(1)
                .WithMessage(x => $"Property: Type | Value: {x.Type}, Tamanho do texto: {x.Type.Length} excede ao permitido: 1");
        }
    }
}
