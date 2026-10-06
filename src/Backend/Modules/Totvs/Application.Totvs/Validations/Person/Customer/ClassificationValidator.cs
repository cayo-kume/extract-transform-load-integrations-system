using Domain.Totvs.Dtos.Person.Customer;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.Customer
{
    public class ClassificationValidator : AbstractValidator<Classification>
    {
        public ClassificationValidator()
        {
            RuleFor(x => x.typeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.typeCode));

            RuleFor(x => x.typeName)
                .MaximumLength(60)
                .WithMessage("O campo 'typeName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeName));

            RuleFor(x => x.code)
                .MaximumLength(60)
                .WithMessage("O campo 'code' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));
        }
    }
}
