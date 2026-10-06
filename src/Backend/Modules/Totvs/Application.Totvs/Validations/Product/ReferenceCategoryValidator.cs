using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class ReferenceCategoryValidator : AbstractValidator<ReferenceCategory>
    {
        public ReferenceCategoryValidator()
        {
            RuleFor(x => x.code)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.type)
                .MaximumLength(60)
                .WithMessage("O campo 'type' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));
        }
    }
}
