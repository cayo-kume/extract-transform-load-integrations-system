using Domain.Movidesk.Dtos.Reads.People;
using FluentValidation;

namespace Application.Movidesk.Validations.People
{
    public class AssetValidator : AbstractValidator<Asset>
    {
        public AssetValidator()
        {
            RuleFor(x => x.id)
                .MaximumLength(64)
                .WithMessage("O campo 'id' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.name)
                .MaximumLength(64)
                .WithMessage("O campo 'name' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.label)
                .MaximumLength(64)
                .WithMessage("O campo 'label' deve ter no máximo 64 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.label));
        }
    }
}
