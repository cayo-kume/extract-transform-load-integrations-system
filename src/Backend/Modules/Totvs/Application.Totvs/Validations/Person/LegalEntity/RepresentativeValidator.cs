using Domain.Totvs.Dtos.Person.LegalEntity;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.LegalEntity
{
    public class RepresentativeValidator : AbstractValidator<Representative>
    {
        public RepresentativeValidator()
        {
            RuleFor(x => x.representativeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.representativeCode));

            RuleFor(x => x.classificationTypeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.classificationTypeCode));

            RuleFor(x => x.cpfCnpj)
                .MaximumLength(14)
                .WithMessage("O campo 'cpfCnpj' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cpfCnpj));

            RuleFor(x => x.classificationCode)
                .MaximumLength(60)
                .WithMessage("O campo 'classificationCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.classificationCode));
        }
    }
}
