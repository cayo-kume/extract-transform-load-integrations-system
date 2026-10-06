using Domain.LinxCommerce.Entities.SalesRepresentative;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.SalesRepresentative
{
    public class SalesRepresentativeContactDataValidator : AbstractValidator<SalesRepresentativeContactData>
    {
        public SalesRepresentativeContactDataValidator()
        {
            RuleFor(x => x.Email)
                .MaximumLength(60)
                .WithMessage(x => $"Property: Email | Value: {x.Email}, Tamanho do texto: {x.Email.Length} excede ao permitido: 60");

            RuleFor(x => x.Phone)
                .MaximumLength(20)
                .WithMessage(x => $"Property: Phone | Value: {x.Phone}, Tamanho do texto: {x.Phone.Length} excede ao permitido: 20");

            RuleFor(x => x.CellPhone)
                .MaximumLength(20)
                .WithMessage(x => $"Property: CellPhone | Value: {x.CellPhone}, Tamanho do texto: {x.CellPhone.Length} excede ao permitido: 20");
        }
    }
}
