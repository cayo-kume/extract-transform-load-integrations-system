using Domain.Totvs.Dtos.Writes.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.AccountsPayable
{
    /// <summary>
    /// valida o corpo do agrupamento de duplicatas no controller (corpo inválido volta 400): a duplicata nova, o vencimento e pelo menos duas
    /// parcelas a agrupar, sem repetição (GroupDuplicateCommand)
    /// </summary>
    public class GroupDuplicateValidator : AbstractValidator<GroupDuplicate>
    {
        public GroupDuplicateValidator()
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
                .WithMessage("O campo 'duplicateCode' é obrigatório (a duplicata nova, que agrupa as parcelas).")
                .InclusiveBetween(1, 9_999_999_999)
                .WithMessage("O campo 'duplicateCode' deve ter de 1 a 10 dígitos.");

            RuleFor(x => x.document)
                .IsInEnum()
                .WithMessage("O campo 'document' só aceita 1 (duplicata) ou 18 (conhecimento de frete).")
                .When(x => x.document is not null);

            RuleFor(x => x.dueDate)
                .NotNull()
                .WithMessage("O campo 'dueDate' é obrigatório.");

            RuleFor(x => x.groupInstallments)
                .Must(x => x is { Count: >= 2 })
                .WithMessage("Informe pelo menos duas parcelas em 'groupInstallments': agrupar uma parcela só não faz sentido.")
                .Must(x => x!.Where(i => i is not null).GroupBy(i => (i.branchCnpj, i.supplierCpfCnpj, i.duplicateCode, i.installmentCode)).All(g => g.Count() == 1))
                .WithMessage("A mesma parcela aparece mais de uma vez em 'groupInstallments'.")
                .When(x => x.groupInstallments is { Count: >= 2 }, ApplyConditionTo.CurrentValidator);

            RuleForEach(x => x.groupInstallments)
                .ChildRules(i =>
                {
                    i.RuleFor(x => x.branchCnpj)
                        .NotEmpty()
                        .WithMessage("O campo 'branchCnpj' da parcela é obrigatório.")
                        .Matches(@"^\d{14}$")
                        .WithMessage("O campo 'branchCnpj' da parcela deve ter 14 dígitos, sem pontuação.");

                    i.RuleFor(x => x.supplierCpfCnpj)
                        .NotEmpty()
                        .WithMessage("O campo 'supplierCpfCnpj' da parcela é obrigatório.")
                        .Matches(@"^(\d{11}|\d{14})$")
                        .WithMessage("O campo 'supplierCpfCnpj' da parcela deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.");

                    i.RuleFor(x => x.duplicateCode)
                        .NotNull()
                        .WithMessage("O campo 'duplicateCode' da parcela é obrigatório.")
                        .InclusiveBetween(1, 9_999_999_999)
                        .WithMessage("O campo 'duplicateCode' da parcela deve ter de 1 a 10 dígitos.");

                    i.RuleFor(x => x.installmentCode)
                        .NotNull()
                        .WithMessage("O campo 'installmentCode' da parcela é obrigatório.")
                        .InclusiveBetween(1, 999)
                        .WithMessage("O campo 'installmentCode' da parcela deve ter de 1 a 3 dígitos.");
                })
                .When(x => x.groupInstallments is not null);
        }
    }
}
