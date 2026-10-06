using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class ManufacturerValidator : AbstractValidator<Manufacturer>
    {
        public ManufacturerValidator()
        {
            RuleFor(x => x.code)
                .MaximumLength(60)
                .WithMessage("O campo 'code' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.cpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));
        }
    }
}
