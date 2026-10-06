using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class DetailValidator : AbstractValidator<Detail>
    {
        public DetailValidator()
        {
            RuleFor(x => x.typeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.typeCode));

            RuleFor(x => x.type)
                .MaximumLength(60)
                .WithMessage("O campo 'type' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.auxiliaryType)
                .MaximumLength(60)
                .WithMessage("O campo 'auxiliaryType' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.auxiliaryType));

            RuleFor(x => x.title)
                .MaximumLength(250)
                .WithMessage("O campo 'title' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.title));

            RuleFor(x => x.description)
                .MaximumLength(300)
                .WithMessage("O campo 'description' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));
        }
    }
}
