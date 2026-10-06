using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.SalesRepresentative
{
    public class SalesRepresentativeValidator : AbstractValidator<Domain.LinxCommerce.Entities.SalesRepresentative.SalesRepresentative>
    {
        public SalesRepresentativeValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(60)
                .WithMessage(x => $"Property: Name | Value: {x.Name}, Tamanho do texto: {x.Name.Length} excede ao permitido: 60");

            RuleFor(x => x.FriendlyCode)
                .MaximumLength(60)
                .WithMessage(x => $"Property: FriendlyCode | Value: {x.FriendlyCode}, Tamanho do texto: {x.FriendlyCode.Length} excede ao permitido: 60");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(800)
                .WithMessage(x => $"Property: ImageUrl | Value: {x.ImageUrl}, Tamanho do texto: {x.ImageUrl.Length} excede ao permitido: 800");

            RuleFor(x => x.IntegrationID)
                .MaximumLength(60)
                .WithMessage(x => $"Property: IntegrationID | Value: {x.IntegrationID}, Tamanho do texto: {x.IntegrationID.Length} excede ao permitido: 60");

            RuleFor(x => x.SalesRepresentativeType)
                .MaximumLength(1)
                .WithMessage(x => $"Property: SalesRepresentativeType | Value: {x.SalesRepresentativeType}, Tamanho do texto: {x.SalesRepresentativeType.Length} excede ao permitido: 1");

            RuleFor(x => x.Status)
                .MaximumLength(1)
                .WithMessage(x => $"Property: Status | Value: {x.Status}, Tamanho do texto: {x.Status.Length} excede ao permitido: 1");

            RuleFor(x => x.Identification)
                .SetValidator(new SalesRepresentativeIdentificationValidator());

            RuleFor(x => x.Commission)
                .SetValidator(new SalesRepresentativeComissionValidator());

            RuleFor(x => x.Contact)
                .SetValidator(new SalesRepresentativeContactDataValidator());

            RuleFor(x => x.ShippingRegion)
                .SetValidator(new SalesRepresentativeShippingRegionValidator());

            RuleForEach(x => x.Addresses)
                .SetValidator(new SalesRepresentativeAddressValidator());

            RuleFor(x => x.MaxDiscount)
                .SetValidator(new SalesRepresentativeMaxDiscountValidator());

            RuleFor(x => x.WebSiteSettings)
                .SetValidator(new SalesRepresentativeWebSiteSettingsValidator());

            RuleFor(x => x.Portfolio)
                .SetValidator(new SalesRepresentativePortfolioValidator());
        }
    }
}
