using Domain.Totvs.Dtos.Writes.AccountsPayable;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.AccountsPayable
{
    /// <summary>
    /// parcela da duplicata: os campos que o swagger declara sem nullable são obrigatórios (omitidos, iriam como 0 ou 0001-01-01)
    /// </summary>
    public class InstallmentValidator : AbstractValidator<Installment>
    {
        private const decimal MaxValue = 999_999_999_999.99m;     // 12 inteiros e 2 decimais
        private const decimal MaxPercentage = 999.9999m;          // 3 inteiros e 4 decimais

        public InstallmentValidator()
        {
            RuleFor(x => x.installmentCode)
                .NotNull()
                .WithMessage("O campo 'installmentCode' é obrigatório.")
                .InclusiveBetween(1, 999)
                .WithMessage("O campo 'installmentCode' deve ter de 1 a 3 dígitos.");

            RuleFor(x => x.bearerCode)
                .NotNull()
                .WithMessage("O campo 'bearerCode' é obrigatório.")
                .InclusiveBetween(0, 9_999)
                .WithMessage("O campo 'bearerCode' deve ter até 4 dígitos.");

            RuleFor(x => x.issueDate).NotNull().WithMessage("O campo 'issueDate' é obrigatório.");
            RuleFor(x => x.arrivalDate).NotNull().WithMessage("O campo 'arrivalDate' é obrigatório.");
            RuleFor(x => x.dueDate)
                .NotNull()
                .WithMessage("O campo 'dueDate' é obrigatório.")
                .GreaterThanOrEqualTo(x => x.issueDate)
                .WithMessage("O campo 'dueDate' não pode ser anterior a 'issueDate'.")
                .When(x => x.issueDate is not null, ApplyConditionTo.CurrentValidator);

            RuleFor(x => x.document)
                .NotNull()
                .WithMessage("O campo 'document' é obrigatório (1 duplicata, 2 boleto, 7 nota fiscal, 18 conhecimento de frete...).")
                .IsInEnum()
                .WithMessage("O campo 'document' só aceita os tipos de documento do swagger (1 a 42 e 50 a 55).");

            RuleFor(x => x.prevision)
                .NotNull()
                .WithMessage("O campo 'prevision' é obrigatório (1 previsão, 2 real, 3 consignação).")
                .IsInEnum()
                .WithMessage("O campo 'prevision' só aceita 1 (previsão), 2 (real) ou 3 (consignação).");

            RuleFor(x => x.stage)
                .NotNull()
                .WithMessage("O campo 'stage' é obrigatório (1 título não conferido, 2 liberado para pagamento, 5 título aceito...).")
                .IsInEnum()
                .WithMessage("O campo 'stage' só aceita os estágios do swagger (1 a 5, 10, 20, 30, 40 a 43, 70, 71 e 90).");

            RuleFor(x => x.duplicateValue)
                .NotNull()
                .WithMessage("O campo 'duplicateValue' é obrigatório.")
                .GreaterThan(0)
                .WithMessage("O campo 'duplicateValue' deve ser maior que zero.")
                .LessThanOrEqualTo(MaxValue)
                .WithMessage("O campo 'duplicateValue' aceita até 12 inteiros.")
                .PrecisionScale(14, 2, true)
                .WithMessage("O campo 'duplicateValue' aceita até 2 casas decimais.");

            foreach (var (field, get) in new (string, Func<Installment, decimal?>)[]
            {
                ("punctualityDiscountValue", x => x.punctualityDiscountValue), ("advanceDiscountValue1", x => x.advanceDiscountValue1),
                ("advanceDiscountValue2", x => x.advanceDiscountValue2), ("assessmentValue", x => x.assessmentValue),
                ("interestDayValue", x => x.interestDayValue), ("expenseFinancial", x => x.expenseFinancial), ("additionValue", x => x.additionValue),
                ("outherAdditionalsValue", x => x.outherAdditionalsValue), ("indemnityValue", x => x.indemnityValue), ("taxValue", x => x.taxValue)
            })
                RuleFor(x => get(x))
                    .InclusiveBetween(0, MaxValue)
                    .WithName(field)
                    .WithMessage($"O campo '{field}' deve ficar entre 0 e 999.999.999.999,99.")
                    .PrecisionScale(14, 2, true)
                    .WithMessage($"O campo '{field}' aceita até 2 casas decimais.")
                    .When(x => get(x) is not null);

            foreach (var (field, get) in new (string, Func<Installment, decimal?>)[]
            {
                ("punctualityDiscountPercentage", x => x.punctualityDiscountPercentage), ("advanceDiscountPercentage1", x => x.advanceDiscountPercentage1),
                ("advanceDiscountPercentage2", x => x.advanceDiscountPercentage2), ("assessmentPercentage", x => x.assessmentPercentage),
                ("interestDayPercentage", x => x.interestDayPercentage), ("taxCalculationBasePercentage", x => x.taxCalculationBasePercentage)
            })
                RuleFor(x => get(x))
                    .InclusiveBetween(0, MaxPercentage)
                    .WithName(field)
                    .WithMessage($"O campo '{field}' deve ficar entre 0 e 999,9999.")
                    .PrecisionScale(7, 4, true)
                    .WithMessage($"O campo '{field}' aceita até 4 casas decimais.")
                    .When(x => get(x) is not null);

            foreach (var (field, get) in new (string, Func<Installment, AdvanceDiscountType?>)[]
            {
                ("advanceDiscount", x => x.advanceDiscount), ("advanceDiscount1", x => x.advanceDiscount1), ("advanceDiscount2", x => x.advanceDiscount2)
            })
                RuleFor(x => get(x))
                    .IsInEnum()
                    .WithName(field)
                    .WithMessage($"O campo '{field}' só aceita de 0 a 7.")
                    .When(x => get(x) is not null);

            RuleFor(x => x.expenses)
                .NotEmpty()
                .WithMessage("Informe pelo menos uma despesa em 'expenses'.");

            RuleForEach(x => x.expenses)
                .ChildRules(expense =>
                {
                    expense.RuleFor(e => e.expenseCode)
                        .NotNull()
                        .WithMessage("O campo 'expenseCode' é obrigatório.")
                        .InclusiveBetween(1, 99_999_999)
                        .WithMessage("O campo 'expenseCode' deve ter de 1 a 8 dígitos.");

                    expense.RuleFor(e => e.costCenterCode)
                        .NotNull()
                        .WithMessage("O campo 'costCenterCode' é obrigatório.")
                        .InclusiveBetween(0, 999_999_999)
                        .WithMessage("O campo 'costCenterCode' deve ter até 9 dígitos.");

                    expense.RuleFor(e => e)
                        .Must(e => (e.proratedPercentage is null) != (e.proratedValue is null))
                        .WithName("proratedPercentage")
                        .WithMessage("Informe exatamente um entre 'proratedPercentage' e 'proratedValue'.");

                    expense.RuleFor(e => e.proratedPercentage)
                        .GreaterThan(0)
                        .WithMessage("O campo 'proratedPercentage' deve ser maior que zero.")
                        .LessThanOrEqualTo(100)
                        .WithMessage("O campo 'proratedPercentage' vai até 100.")
                        .PrecisionScale(7, 4, true)
                        .WithMessage("O campo 'proratedPercentage' aceita até 4 casas decimais.")
                        .When(e => e.proratedPercentage is not null);

                    expense.RuleFor(e => e.proratedValue)
                        .GreaterThan(0)
                        .WithMessage("O campo 'proratedValue' deve ser maior que zero.")
                        .LessThanOrEqualTo(MaxValue)
                        .WithMessage("O campo 'proratedValue' aceita até 12 inteiros.")
                        .PrecisionScale(14, 2, true)
                        .WithMessage("O campo 'proratedValue' aceita até 2 casas decimais.")
                        .When(e => e.proratedValue is not null);
                })
                .When(x => x.expenses is not null);

            // rateio só por percentual: a soma da parcela fecha em 100
            RuleFor(x => x.expenses)
                .Must(e => e!.Sum(x => x.proratedPercentage!.Value) == 100)
                .WithMessage("Os percentuais de rateio ('proratedPercentage') das despesas da parcela devem somar 100.")
                .When(x => x.expenses is { Count: > 0 } && x.expenses.All(e => e is not null && e.proratedPercentage is not null && e.proratedValue is null));

            RuleForEach(x => x.observations)
                .ChildRules(line =>
                    line.RuleFor(l => l.observation)
                        .NotEmpty()
                        .WithMessage("O campo 'observation' é obrigatório.")
                        .MaximumLength(80)
                        .WithMessage("O campo 'observation' aceita até 80 caracteres."))
                .When(x => x.observations is not null);

            RuleFor(x => x.billingAccept!.assignorCnpj)
                .Matches(@"^\d{14}$")
                .WithMessage("O campo 'billingAccept.assignorCnpj' deve ter 14 dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.billingAccept?.assignorCnpj));

            RuleForEach(x => x.classifications)
                .ChildRules(c =>
                {
                    c.RuleFor(x => x.classificationType)
                        .NotNull()
                        .WithMessage("O campo 'classificationType' é obrigatório.")
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'classificationType' deve ter de 1 a 4 dígitos.");

                    c.RuleFor(x => x.classificationCode)
                        .NotEmpty()
                        .WithMessage("O campo 'classificationCode' é obrigatório.")
                        .MaximumLength(10)
                        .WithMessage("O campo 'classificationCode' aceita até 10 caracteres.");
                })
                .When(x => x.classifications is not null);

            RuleForEach(x => x.additionalFields)
                .ChildRules(f =>
                {
                    f.RuleFor(x => x.typeFieldCode)
                        .NotNull()
                        .WithMessage("O campo 'typeFieldCode' é obrigatório.")
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'typeFieldCode' deve ter de 1 a 4 dígitos.");

                    f.RuleFor(x => x)
                        .Must(x => new object?[] { x.descriptionField, x.numericField, x.dateField }.Count(v => v is not null) == 1)
                        .WithName("descriptionField")
                        .WithMessage("Informe o dado do campo adicional em exatamente um entre 'descriptionField', 'numericField' e 'dateField'.");

                    f.RuleFor(x => x.descriptionField)
                        .MaximumLength(80)
                        .WithMessage("O campo 'descriptionField' aceita até 80 caracteres.");
                })
                .When(x => x.additionalFields is not null);

            RuleFor(x => x.taxAdditional)
                .ChildRules(t =>
                {
                    t.RuleFor(x => x.revenue)
                        .NotNull()
                        .WithMessage("O campo 'taxAdditional.revenue' é obrigatório.")
                        .InclusiveBetween(0, 999_999)
                        .WithMessage("O campo 'taxAdditional.revenue' deve ter até 6 dígitos.");

                    t.RuleFor(x => x.calculationPeriod).NotNull().WithMessage("O campo 'taxAdditional.calculationPeriod' é obrigatório.");
                    t.RuleFor(x => x.competenceDate).NotNull().WithMessage("O campo 'taxAdditional.competenceDate' é obrigatório.");
                    t.RuleFor(x => x.isThirteenthSalary).NotNull().WithMessage("O campo 'taxAdditional.isThirteenthSalary' é obrigatório.");

                    t.RuleFor(x => x.reference)
                        .MaximumLength(20)
                        .WithMessage("O campo 'taxAdditional.reference' aceita até 20 caracteres.");
                })
                .When(x => x.taxAdditional is not null);

            RuleForEach(x => x.taxDuplicates)
                .ChildRules(t =>
                {
                    t.RuleFor(x => x.tax)
                        .NotNull()
                        .WithMessage("O campo 'tax' é obrigatório.")
                        .InclusiveBetween(1, 99)
                        .WithMessage("O campo 'tax' deve ter de 1 a 2 dígitos.");

                    t.RuleFor(x => x.ratePercentage)
                        .NotNull()
                        .WithMessage("O campo 'ratePercentage' é obrigatório.")
                        .InclusiveBetween(0, 999.999999m)
                        .WithMessage("O campo 'ratePercentage' deve ficar entre 0 e 999,999999.")
                        .PrecisionScale(9, 6, true)
                        .WithMessage("O campo 'ratePercentage' aceita até 6 casas decimais.");

                    t.RuleFor(x => x.taxValue)
                        .NotNull()
                        .WithMessage("O campo 'taxValue' do imposto é obrigatório.")
                        .InclusiveBetween(0, MaxValue)
                        .WithMessage("O campo 'taxValue' do imposto deve ficar entre 0 e 999.999.999.999,99.")
                        .PrecisionScale(14, 2, true)
                        .WithMessage("O campo 'taxValue' do imposto aceita até 2 casas decimais.");

                    t.RuleFor(x => x.status)
                        .NotNull()
                        .WithMessage("O campo 'status' do imposto é obrigatório (0 não retido, 1 retido na própria duplicata, 2 retido em outra).")
                        .IsInEnum()
                        .WithMessage("O campo 'status' do imposto só aceita 0, 1 ou 2.");

                    t.RuleFor(x => x.retentionType)
                        .IsInEnum()
                        .WithMessage("O campo 'retentionType' só aceita de 1 a 6.")
                        .When(x => x.retentionType is not null);

                    t.RuleFor(x => x.branchCnpj)
                        .Matches(@"^\d{14}$")
                        .WithMessage("O campo 'branchCnpj' do imposto deve ter 14 dígitos, sem pontuação.")
                        .When(x => !String.IsNullOrEmpty(x.branchCnpj));

                    t.RuleFor(x => x.suplierCpfCnpj)
                        .Matches(@"^(\d{11}|\d{14})$")
                        .WithMessage("O campo 'suplierCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                        .When(x => !String.IsNullOrEmpty(x.suplierCpfCnpj));

                    t.RuleFor(x => x.dirfRevenue)
                        .MaximumLength(4)
                        .WithMessage("O campo 'dirfRevenue' aceita até 4 caracteres.");
                })
                .When(x => x.taxDuplicates is not null);

            RuleForEach(x => x.businessUnits)
                .ChildRules(b =>
                {
                    b.RuleFor(x => x.classificationCode)
                        .NotEmpty()
                        .WithMessage("O campo 'classificationCode' da unidade de negócio é obrigatório.")
                        .MaximumLength(10)
                        .WithMessage("O campo 'classificationCode' da unidade de negócio aceita até 10 caracteres.");

                    b.RuleFor(x => x.proratedPercentage)
                        .NotNull()
                        .WithMessage("O campo 'proratedPercentage' da unidade de negócio é obrigatório.")
                        .InclusiveBetween(0, 100)
                        .WithMessage("O campo 'proratedPercentage' da unidade de negócio vai de 0 a 100.");
                })
                .When(x => x.businessUnits is not null);
        }
    }
}
