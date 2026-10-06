using Domain.LinxCommerce.Entities.SalesRepresentative;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.SalesRepresentative
{
    public class SalesRepresentativeShippingRegionValidator : AbstractValidator<SalesRepresentativeShippingRegion>
    {
        public SalesRepresentativeShippingRegionValidator()
        {
            RuleFor(x => x.SelectedMode)
                .MaximumLength(1)
                .WithMessage(x => $"Property: SelectedMode | Value: {x.SelectedMode}, Tamanho do texto: {x.SelectedMode.Length} excede ao permitido: 1");
        }
    }
}
