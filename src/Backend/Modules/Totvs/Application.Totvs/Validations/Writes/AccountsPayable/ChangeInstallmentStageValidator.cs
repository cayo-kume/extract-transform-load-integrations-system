using Domain.Totvs.Dtos.Writes.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.AccountsPayable
{
    /// <summary>
    /// valida o corpo da troca de estágio no controller, antes de chamar a Totvs (corpo inválido volta 400). Obrigatórios e
    /// tamanhos do swagger da API Accounts Payable (ChangeInstallmentStageCommand) e a regra "um ou outro" do fornecedor
    /// </summary>
    public class ChangeInstallmentStageValidator : AbstractValidator<ChangeInstallmentStage>
    {
        public ChangeInstallmentStageValidator()
        {
            RuleFor(x => x.branchCnpj)
                .NotEmpty()
                .WithMessage("O campo 'branchCnpj' é obrigatório.")
                .Matches(@"^\d{14}$")
                .WithMessage("O campo 'branchCnpj' deve ter 14 dígitos, sem pontuação.");

            RuleFor(x => x)
                .Must(x => String.IsNullOrEmpty(x.supplierCpfCnpj) != (x.supplierCode is null))
                .WithName("supplierCpfCnpj")
                .WithMessage("Informe exatamente um entre 'supplierCpfCnpj' e 'supplierCode'.");

            RuleFor(x => x.supplierCpfCnpj)
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'supplierCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.supplierCpfCnpj));

            RuleFor(x => x.supplierCode)
                .GreaterThan(0)
                .WithMessage("O campo 'supplierCode' deve ser maior que zero.")
                .When(x => x.supplierCode is not null);

            RuleFor(x => x.duplicateCode)
                .NotNull()
                .WithMessage("O campo 'duplicateCode' é obrigatório.")
                .InclusiveBetween(1, 9_999_999_999)
                .WithMessage("O campo 'duplicateCode' deve ter de 1 a 10 dígitos.");

            RuleFor(x => x.installmentCode)
                .NotNull()
                .WithMessage("O campo 'installmentCode' é obrigatório.")
                .InclusiveBetween(1, 999)
                .WithMessage("O campo 'installmentCode' deve ter de 1 a 3 dígitos.");

            RuleFor(x => x.newStage)
                .NotNull()
                .WithMessage("O campo 'newStage' é obrigatório.")
                .IsInEnum()
                .WithMessage("O campo 'newStage' só aceita 1 (título não conferido), 2 (liberado para pagamento) ou 5 (título aceito).");
        }
    }
}
