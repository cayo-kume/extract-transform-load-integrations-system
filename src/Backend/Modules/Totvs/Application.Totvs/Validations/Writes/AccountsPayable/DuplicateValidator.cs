using Domain.Totvs.Dtos.Writes.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.AccountsPayable
{
    /// <summary>
    /// valida o corpo da inclusão de duplicata no controller (corpo inválido volta 400): obrigatórios e tamanhos do swagger
    /// (CreatedDuplicateCommand), parcelas sem repetição, cada uma com pelo menos uma despesa, rateio por percentual ou por valor e, por
    /// percentual, somando 100
    /// </summary>
    public class DuplicateValidator : AbstractValidator<Duplicate>
    {
        public DuplicateValidator()
        {
            RuleFor(x => x.branchCnpj)
                .NotEmpty()
                .WithMessage("O campo 'branchCnpj' é obrigatório.")
                .Matches(@"^\d{14}$")
                .WithMessage("O campo 'branchCnpj' deve ter 14 dígitos, sem pontuação.");

            RuleFor(x => x.supplierCpfCnpj)
                .NotEmpty()
                .WithMessage("O campo 'supplierCpfCnpj' é obrigatório.")
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'supplierCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.");

            RuleFor(x => x.duplicateCode)
                .NotNull()
                .WithMessage("O campo 'duplicateCode' é obrigatório.")
                .InclusiveBetween(1, 9_999_999_999)
                .WithMessage("O campo 'duplicateCode' deve ter de 1 a 10 dígitos.");

            RuleFor(x => x.installments)
                .NotEmpty()
                .WithMessage("Informe pelo menos uma parcela em 'installments'.")
                .Must(x => x!.Where(i => i?.installmentCode is not null).GroupBy(i => i.installmentCode).All(g => g.Count() == 1))
                .WithMessage("A mesma parcela ('installmentCode') aparece mais de uma vez em 'installments'.")
                .When(x => x.installments is { Count: > 0 }, ApplyConditionTo.CurrentValidator);

            RuleForEach(x => x.installments)
                .SetValidator(new InstallmentValidator())
                .When(x => x.installments is not null);
        }
    }
}
