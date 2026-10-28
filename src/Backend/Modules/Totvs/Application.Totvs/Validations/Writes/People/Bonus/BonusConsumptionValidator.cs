using Domain.Totvs.Dtos.Writes.People.Bonus;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Bonus
{
    /// <summary>
    /// valida o corpo do consumo de bônus no controller (corpo inválido volta 400): a pessoa por código ou por CPF/CNPJ, a loja e o valor
    /// (até 9 inteiros e 2 decimais)
    /// </summary>
    public class BonusConsumptionValidator : AbstractValidator<BonusConsumption>
    {
        public BonusConsumptionValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.personCode is null) != String.IsNullOrEmpty(x.personCpfCnpj))
                .WithName("personCode")
                .WithMessage("Informe exatamente um entre 'personCode' e 'personCpfCnpj'.");

            RuleFor(x => x.personCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'personCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.personCode is not null);

            RuleFor(x => x.personCpfCnpj)
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'personCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.personCpfCnpj));

            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.usedValue)
                .NotNull()
                .WithMessage("O campo 'usedValue' é obrigatório.")
                .GreaterThan(0)
                .WithMessage("O campo 'usedValue' deve ser maior que zero.")
                .LessThanOrEqualTo(999_999_999.99m)
                .WithMessage("O campo 'usedValue' aceita até 9 inteiros.")
                .PrecisionScale(11, 2, true)
                .WithMessage("O campo 'usedValue' aceita até 2 casas decimais.");
        }
    }
}
