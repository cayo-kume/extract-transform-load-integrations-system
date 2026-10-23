using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal onde ele informa, senão os conferidos numa amostra real
    /// (colunas do Script025_Create_Totvs_Invoices_Tables)
    /// </summary>
    public class ElectronicValidator : AbstractValidator<Electronic>
    {
        public ElectronicValidator()
        {
            RuleFor(x => x.accessKey)
                .MaximumLength(50)
                .WithMessage("O campo 'accessKey' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.accessKey));

            RuleFor(x => x.electronicInvoiceStatus)
                .MaximumLength(20)
                .WithMessage("O campo 'electronicInvoiceStatus' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.electronicInvoiceStatus));

            RuleFor(x => x.receipt)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.receipt));

            RuleFor(x => x.receivementDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.receivementDate));

            RuleFor(x => x.disableProtocol)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.disableProtocol));

            RuleFor(x => x.disableDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.disableDate));

            RuleFor(x => x.nfseCode)
                .MaximumLength(60)
                .WithMessage("O campo 'nfseCode' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nfseCode));
        }
    }
}
