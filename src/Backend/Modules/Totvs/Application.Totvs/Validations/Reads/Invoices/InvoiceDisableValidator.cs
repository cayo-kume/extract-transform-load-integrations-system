using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Invoices;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Invoices
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Fiscal (InvoiceDisableDataModel)
    /// </summary>
    public class InvoiceDisableValidator : AbstractValidator<InvoiceDisable>
    {
        public InvoiceDisableValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.initialInvoiceNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'initialInvoiceNumber' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.finalInvoiceNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'finalInvoiceNumber' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x)
                .Must(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.initialInvoiceNumber) <= ConverterValidatorsExtensions.ConvertToInt32Validation(x.finalInvoiceNumber))
                .WithMessage("O 'initialInvoiceNumber' é maior que o 'finalInvoiceNumber'.")
                .When(x => !string.IsNullOrEmpty(x.initialInvoiceNumber) && !string.IsNullOrEmpty(x.finalInvoiceNumber));

            RuleFor(x => x.branchCnpj)
                .Matches(@"^\d{14}$")
                .WithMessage("O campo 'branchCnpj' deve ter 14 dígitos.")
                .When(x => !string.IsNullOrEmpty(x.branchCnpj));

            RuleFor(x => x.serialCode)
                .MaximumLength(3)
                .WithMessage("O campo 'serialCode' deve ter no máximo 3 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serialCode));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
