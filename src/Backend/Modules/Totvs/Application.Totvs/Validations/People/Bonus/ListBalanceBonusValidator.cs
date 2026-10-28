using Domain.Totvs.Dtos.Reads.People.Bonus;
using FluentValidation;

namespace Application.Totvs.Validations.People.Bonus
{
    /// <summary>
    /// valida a consulta de saldo de bônus antes de chamar a Totvs (docs/PLANO-TOTVS-METODOS-FALTANTES.md §3.3); as lojas da empresa são
    /// conferidas no service
    /// </summary>
    public class ListBalanceBonusValidator : AbstractValidator<ListBalanceBonus>
    {
        public ListBalanceBonusValidator()
        {
            RuleFor(x => x.personCpf)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'personCpf' é obrigatório.")
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'personCpf' deve ter 11 ou 14 dígitos.");

            RuleFor(x => x.branchList)
                .NotEmpty()
                .WithMessage("Informe ao menos uma loja em 'branchList'.");

            RuleForEach(x => x.branchList)
                .Must(x => (x.branchCode is > 0) != !String.IsNullOrEmpty(x.branchCnpj))
                .WithMessage("Cada loja de 'branchList' leva branchCode ou branchCnpj, um dos dois.");

            RuleForEach(x => x.branchList)
                .Must(x => String.IsNullOrEmpty(x.branchCnpj) || System.Text.RegularExpressions.Regex.IsMatch(x.branchCnpj, @"^\d{14}$"))
                .WithMessage("O 'branchCnpj' de 'branchList' deve ter 14 dígitos.");
        }
    }
}
