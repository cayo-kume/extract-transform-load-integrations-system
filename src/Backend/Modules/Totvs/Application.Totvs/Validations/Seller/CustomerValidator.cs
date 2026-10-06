using FluentValidation;
using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Seller;

namespace Application.Totvs.Validations.Seller
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.personCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personCode));

            RuleFor(x => x.document)
                .MaximumLength(14)
                .WithMessage("O campo 'document' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.document));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));
        }
    }
}
