using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script023_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class DigitalWalletInformationValidator : AbstractValidator<DigitalWalletInformation>
    {
        public DigitalWalletInformationValidator()
        {
            RuleFor(x => x.nsu)
                .MaximumLength(100)
                .WithMessage("O campo 'nsu' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nsu));

            RuleFor(x => x.transactionId)
                .MaximumLength(100)
                .WithMessage("O campo 'transactionId' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.transactionId));

            RuleFor(x => x.processorId)
                .MaximumLength(100)
                .WithMessage("O campo 'processorId' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.processorId));
        }
    }
}
