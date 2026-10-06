using Domain.Totvs.Dtos.Fiscal;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Fiscal
{
    public class DigitalWalletInformationValidator : AbstractValidator<DigitalWalletInformation>
    {
        public DigitalWalletInformationValidator()
        {
            RuleFor(x => x.nsu)
                .MaximumLength(60)
                .WithMessage("O campo 'nsu' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nsu));

            RuleFor(x => x.transactionId)
                .MaximumLength(60)
                .WithMessage("O campo 'transactionId' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.transactionId));

            RuleFor(x => x.processorId)
                .MaximumLength(60)
                .WithMessage("O campo 'processorId' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.processorId));
        }
    }
}
