using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class SupplierValidator : AbstractValidator<Supplier>
    {
        public SupplierValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.isDefault)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isDefault));

            RuleFor(x => x.cpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.originalCode)
                .MaximumLength(60)
                .WithMessage("O campo 'originalCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.originalCode));
        }
    }
}
