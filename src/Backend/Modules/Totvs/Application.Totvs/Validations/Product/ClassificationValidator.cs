using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
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

			RuleFor(x => x.typeNameAux)
				.MaximumLength(60)
				.WithMessage("O campo 'typeNameAux' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.typeNameAux));

			RuleFor(x => x.code)
				.MaximumLength(60)
				.WithMessage("O campo 'code' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.code));

			RuleFor(x => x.name)
				.MaximumLength(60)
				.WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.name));

			RuleFor(x => x.nameAux)
				.MaximumLength(60)
				.WithMessage("O campo 'nameAux' deve ter no máximo 60 caracteres.")
				.When(x => !string.IsNullOrEmpty(x.nameAux));
		}
    }
}
