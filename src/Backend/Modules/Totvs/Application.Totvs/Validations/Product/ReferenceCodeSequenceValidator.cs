using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class ReferenceCodeSequenceValidator : AbstractValidator<ReferenceCodeSequence>
    {
        public ReferenceCodeSequenceValidator()
        {
            RuleFor(x => x.parentCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.parentCode));

            RuleFor(x => x.referenceId)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.referenceId));

            RuleFor(x => x.code)
                .MaximumLength(60)
                .WithMessage("O campo 'code' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.name)
                .MaximumLength(250)
                .WithMessage("O campo 'name' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));
        }
    }
}
