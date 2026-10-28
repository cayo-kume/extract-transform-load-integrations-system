using Domain.Totvs.Dtos.Writes.People.Bonus;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Bonus
{
    /// <summary>
    /// valida o corpo do crédito de bônus no controller (corpo inválido volta 400): obrigatórios do swagger, valor positivo, utilizado até o
    /// valor, validade em ordem e ainda vigente, tipo de 1 a 15 e cada grupo de origem completo (transação, fatura, liquidação)
    /// </summary>
    public class BonusDueDateValidator : AbstractValidator<BonusDueDate>
    {
        public BonusDueDateValidator()
        {
            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.personCode)
                .NotNull()
                .WithMessage("O campo 'personCode' é obrigatório.")
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'personCode' deve ter de 1 a 9 dígitos.");

            RuleFor(x => x.bonusType)
                .NotNull()
                .WithMessage("O campo 'bonusType' é obrigatório (1 a 15).")
                .InclusiveBetween(1, 15)
                .WithMessage("O campo 'bonusType' só aceita de 1 a 15.");

            RuleFor(x => x.movementDate).NotNull().WithMessage("O campo 'movementDate' é obrigatório.");
            RuleFor(x => x.startDate).NotNull().WithMessage("O campo 'startDate' é obrigatório.");

            RuleFor(x => x.endDate)
                .NotNull()
                .WithMessage("O campo 'endDate' é obrigatório.")
                .GreaterThanOrEqualTo(x => x.startDate)
                .WithMessage("O campo 'endDate' não pode ser anterior a 'startDate'.")
                .When(x => x.startDate is not null, ApplyConditionTo.CurrentValidator)
                .Must(d => d!.Value.Date >= DateTime.Today)
                .WithMessage("O campo 'endDate' não pode ser anterior a hoje: o bônus nasceria vencido.")
                .When(x => x.endDate is not null, ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.bonusValue)
                .NotNull()
                .WithMessage("O campo 'bonusValue' é obrigatório.")
                .GreaterThan(0)
                .WithMessage("O campo 'bonusValue' deve ser maior que zero.")
                .LessThanOrEqualTo(99_999_999_999_999.99m)
                .WithMessage("O campo 'bonusValue' aceita até 14 inteiros.")
                .PrecisionScale(16, 2, true)
                .WithMessage("O campo 'bonusValue' aceita até 2 casas decimais.");

            RuleFor(x => x.usedValue)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O campo 'usedValue' não pode ser negativo.")
                .LessThanOrEqualTo(x => x.bonusValue)
                .WithMessage("O campo 'usedValue' não pode passar do 'bonusValue'.")
                .PrecisionScale(16, 2, true)
                .WithMessage("O campo 'usedValue' aceita até 2 casas decimais.")
                .When(x => x.usedValue is not null);

            RuleFor(x => x)
                .Must(x => new object?[] { x.transactionBranchCode, x.transactionDate, x.transactionNumber }.All(v => v is null) || new object?[] { x.transactionBranchCode, x.transactionDate, x.transactionNumber }.All(v => v is not null))
                .WithName("transactionBranchCode")
                .WithMessage("A transação de origem vai completa: 'transactionBranchCode', 'transactionDate' e 'transactionNumber'.");

            RuleFor(x => x)
                .Must(x => new object?[] { x.invoiceBranchCode, x.invoiceClientCode, x.invoiceNumber, x.installmentNumber }.All(v => v is null) || new object?[] { x.invoiceBranchCode, x.invoiceClientCode, x.invoiceNumber, x.installmentNumber }.All(v => v is not null))
                .WithName("invoiceBranchCode")
                .WithMessage("A fatura de origem vai completa: 'invoiceBranchCode', 'invoiceClientCode', 'invoiceNumber' e 'installmentNumber'.");

            RuleFor(x => x)
                .Must(x => new object?[] { x.settlementBranchCode, x.settlementDate, x.settlementSequenceNumber }.All(v => v is null) || new object?[] { x.settlementBranchCode, x.settlementDate, x.settlementSequenceNumber }.All(v => v is not null))
                .WithName("settlementBranchCode")
                .WithMessage("A liquidação de origem vai completa: 'settlementBranchCode', 'settlementDate' e 'settlementSequenceNumber'.");

            RuleFor(x => x)
                .Must(x => new int?[] { x.transactionBranchCode, x.invoiceBranchCode, x.settlementBranchCode }.All(b => b is null or >= 1 and <= 9_999)
                    && x.transactionNumber is null or >= 1 and <= 999_999_999 && x.invoiceClientCode is null or >= 1 and <= 999_999_999
                    && x.installmentNumber is null or >= 1 and <= 999 && x.settlementSequenceNumber is null or >= 1 and <= 999_999_999)
                .WithName("transactionNumber")
                .WithMessage("Os códigos da origem respeitam os tamanhos do swagger (lojas 4, transação, cliente e sequência 9, parcela 3 dígitos).");
        }
    }
}
