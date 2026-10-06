using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class SatValidator : AbstractValidator<Sat>
    {
        public SatValidator()
        {
            RuleFor(x => x.satCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.satCode));

            RuleFor(x => x.dvNumber)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.dvNumber));

            RuleFor(x => x.cnfNumber)
                .MaximumLength(60)
                .WithMessage("O campo 'cnfNumber' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnfNumber));

            RuleFor(x => x.cancellationKey)
                .MaximumLength(60)
                .WithMessage("O campo 'cancellationKey' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cancellationKey));

            RuleFor(x => x.satSequence)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.satSequence));

            RuleFor(x => x.serialMachine)
                .MaximumLength(60)
                .WithMessage("O campo 'serialMachine' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serialMachine));
        }
    }
}
