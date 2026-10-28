using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// limite de crédito por loja (LimitInDto): efeito financeiro: tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class LimitInValidator : AbstractValidator<LimitIn>
    {
        public LimitInValidator()
        {
            RuleFor(x => x.branchCode)
                .InclusiveBetween(0, 9_999)
                .WithMessage("O campo 'branchCode' deve ter até 4 dígitos (0 zera).")
                .When(x => x.branchCode is not null);

            RuleFor(x => x.limitValue)
                .InclusiveBetween(0, 99_999_999_999_999.99m)
                .WithMessage("O campo 'limitValue' aceita até 14 inteiros (0 zera).")
                .PrecisionScale(16, 2, true)
                .WithMessage("O campo 'limitValue' aceita até 2 casas decimais.")
                .When(x => x.limitValue is not null);

            RuleFor(x => x.saleLimitValue)
                .InclusiveBetween(0, 99_999_999_999_999.99m)
                .WithMessage("O campo 'saleLimitValue' aceita até 14 inteiros (0 zera).")
                .PrecisionScale(16, 2, true)
                .WithMessage("O campo 'saleLimitValue' aceita até 2 casas decimais.")
                .When(x => x.saleLimitValue is not null);

            RuleFor(x => x.monthlyLimitValue)
                .InclusiveBetween(0, 99_999_999_999_999.99m)
                .WithMessage("O campo 'monthlyLimitValue' aceita até 14 inteiros (0 zera).")
                .PrecisionScale(16, 2, true)
                .WithMessage("O campo 'monthlyLimitValue' aceita até 2 casas decimais.")
                .When(x => x.monthlyLimitValue is not null);

            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' do limite é obrigatório (loja da empresa).")
                .GreaterThan(0)
                .WithMessage("O campo 'branchCode' do limite deve ser maior que zero.");
        }
    }
}
