using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// classificação (ClassificationInDto): a lista só inclui; para tirar, PostRemoveClassificationRelationship: tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class ClassificationInValidator : AbstractValidator<ClassificationIn>
    {
        public ClassificationInValidator()
        {
            RuleFor(x => x.classificationTypeCode)
                .InclusiveBetween(0, 9_999)
                .WithMessage("O campo 'classificationTypeCode' deve ter até 4 dígitos (0 zera).")
                .When(x => x.classificationTypeCode is not null);

            RuleFor(x => x.classificationCode)
                .MaximumLength(10)
                .WithMessage("O campo 'classificationCode' aceita até 10 caracteres.");

            RuleFor(x => x.classificationTypeCode)
                .NotNull()
                .WithMessage("O campo 'classificationTypeCode' é obrigatório.")
                .GreaterThan(0)
                .WithMessage("O campo 'classificationTypeCode' deve ser maior que zero.");

            RuleFor(x => x.classificationCode)
                .NotEmpty()
                .WithMessage("O campo 'classificationCode' é obrigatório.");
        }
    }
}
