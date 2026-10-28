using Application.Totvs.Validations.Writes.People.Customers;
using Domain.Totvs.Dtos.Writes.PurchaseOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.PurchaseOrders
{
    /// <summary>
    /// valida o corpo da inclusão de pedido de compra no controller (corpo inválido volta 400): obrigatórios e tamanhos do swagger, fornecedor e
    /// comprador por código ou documento, no máximo uma transportadora, o supplierOrderCode (obrigatório aqui, DP2) e pelo menos um item. O
    /// total contra os itens, a soma dos rateios e os tipos de valor do parâmetro DS_LST_VLR_PED_COMPRA ficam com a Totvs
    /// </summary>
    public class InsertPurchaseOrderValidator : AbstractValidator<InsertPurchaseOrder>
    {
        public InsertPurchaseOrderValidator()
        {
            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x)
                .Must(x => (x.supplierCode is null) != String.IsNullOrEmpty(x.supplierCpfCnpj))
                .WithName("supplierCode")
                .WithMessage("Informe exatamente um entre 'supplierCode' e 'supplierCpfCnpj'.");

            RuleFor(x => x)
                .Must(x => (x.buyerCode is null) != String.IsNullOrEmpty(x.buyerCpfCnpj))
                .WithName("buyerCode")
                .WithMessage("Informe exatamente um entre 'buyerCode' e 'buyerCpfCnpj'.");

            RuleFor(x => x)
                .Must(x => x.shippingCompanyCode is null || String.IsNullOrEmpty(x.shippingCompanyCpfCnpj))
                .WithName("shippingCompanyCode")
                .WithMessage("Informe no máximo um entre 'shippingCompanyCode' e 'shippingCompanyCpfCnpj'.");

            foreach (var (field, get) in new (string, Func<InsertPurchaseOrder, string?>)[]
            {
                ("supplierCpfCnpj", x => x.supplierCpfCnpj), ("buyerCpfCnpj", x => x.buyerCpfCnpj), ("shippingCompanyCpfCnpj", x => x.shippingCompanyCpfCnpj)
            })
                RuleFor(x => get(x))
                    .Matches(@"^(\d{11}|\d{14})$")
                    .WithName(field)
                    .WithMessage($"O campo '{field}' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                    .When(x => !String.IsNullOrEmpty(get(x)));

            foreach (var (field, get) in new (string, Func<InsertPurchaseOrder, int?>)[]
            {
                ("supplierCode", x => x.supplierCode), ("buyerCode", x => x.buyerCode), ("shippingCompanyCode", x => x.shippingCompanyCode)
            })
                RuleFor(x => get(x))
                    .InclusiveBetween(1, 999_999_999)
                    .WithName(field)
                    .WithMessage($"O campo '{field}' deve ter de 1 a 9 dígitos.")
                    .When(x => get(x) is not null);

            RuleFor(x => x.operationCode)
                .NotNull()
                .WithMessage("O campo 'operationCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'operationCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.paymentConditionCode)
                .NotNull()
                .WithMessage("O campo 'paymentConditionCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'paymentConditionCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.status)
                .NotNull()
                .WithMessage("O campo 'status' é obrigatório (1 em andamento, 5 bloqueado).")
                .IsInEnum()
                .WithMessage("O campo 'status' só aceita 1 (em andamento) ou 5 (bloqueado).");

            RuleFor(x => x.supplierOrderCode)
                .NotEmpty()
                .WithMessage("O campo 'supplierOrderCode' é obrigatório: é por ele que uma inclusão repetida é recusada.")
                .MaximumLength(20)
                .WithMessage("O campo 'supplierOrderCode' aceita até 20 caracteres.");

            RuleFor(x => x.paymentType).IsInEnum().WithMessage("O campo 'paymentType' só aceita 1 (duplicata), 2 (boleto) ou 12 (depósito).");
            RuleFor(x => x.freightType).IsInEnum().WithMessage("O campo 'freightType' só aceita de 1 a 6.");

            RuleFor(x => x.totalAmountOrder)
                .NotNull()
                .WithMessage("O campo 'totalAmountOrder' é obrigatório (a Totvs confere com os itens).")
                .InclusiveBetween(0, 999_999_999_999.99m)
                .WithMessage("O campo 'totalAmountOrder' aceita até 12 inteiros.")
                .PrecisionScale(14, 2, true)
                .WithMessage("O campo 'totalAmountOrder' aceita até 2 casas decimais.");

            RuleFor(x => x.items)
                .NotEmpty()
                .WithMessage("Informe pelo menos um item em 'items'.");

            RuleFor(x => x.items)
                .Must(l => l!.Where(i => i is not null).GroupBy(i => i.productCode?.ToString() ?? i.productSku).All(g => g.Count() == 1))
                .WithMessage("O mesmo produto aparece mais de uma vez em 'items'.")
                .When(x => x.items is { Count: > 0 });

            RuleForEach(x => x.items)
                .SetValidator(new PurchaseOrderItemValidator())
                .When(x => x.items is not null);

            RuleForEach(x => x.classifications)
                .ChildRules(c =>
                {
                    c.RuleFor(x => x)
                        .Must(x => (x.classificationTypeCode is null) == String.IsNullOrEmpty(x.classificationCode))
                        .WithName("classificationTypeCode")
                        .WithMessage("A classificação vai com 'classificationTypeCode' e 'classificationCode', os dois.");

                    c.RuleFor(x => x.classificationTypeCode)
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'classificationTypeCode' deve ter de 1 a 4 dígitos.")
                        .When(x => x.classificationTypeCode is not null);

                    c.RuleFor(x => x.classificationCode).MaximumLength(10).WithMessage("O campo 'classificationCode' aceita até 10 caracteres.");
                })
                .When(x => x.classifications is not null);

            RuleForEach(x => x.observations)
                .ChildRules(o =>
                {
                    o.RuleFor(x => x.observation)
                        .NotEmpty()
                        .WithMessage("O campo 'observation' é obrigatório.")
                        .MaximumLength(80)
                        .WithMessage("O campo 'observation' aceita até 80 caracteres.");

                    o.RuleFor(x => x.visualizationType).IsInEnum().WithMessage("O campo 'visualizationType' só aceita 1 (interna), 2 (fornecedor) ou 3 (alerta).");
                })
                .When(x => x.observations is not null);

            RuleForEach(x => x.expenses)
                .SetValidator(new PurchaseOrderExpenseValidator())
                .When(x => x.expenses is not null);

            RuleForEach(x => x.discounts)
                .ChildRules(d =>
                {
                    d.RuleFor(x => x.discountTypeCode)
                        .NotNull()
                        .WithMessage("O campo 'discountTypeCode' é obrigatório.")
                        .InclusiveBetween(1, 999)
                        .WithMessage("O campo 'discountTypeCode' deve ter de 1 a 3 dígitos.");

                    d.RuleFor(x => x.discountPercentage)
                        .InclusiveBetween(0, 100)
                        .WithMessage("O campo 'discountPercentage' do desconto vai de 0 a 100.")
                        .PrecisionScale(9, 6, true)
                        .WithMessage("O campo 'discountPercentage' do desconto aceita até 6 casas decimais.")
                        .When(x => x.discountPercentage is not null);
                })
                .When(x => x.discounts is not null);

            RuleForEach(x => x.additionalFields)
                .ChildRules(f =>
                {
                    f.RuleFor(x => x.code)
                        .NotNull()
                        .WithMessage("O campo 'code' do campo adicional é obrigatório.")
                        .GreaterThan(0)
                        .WithMessage("O campo 'code' do campo adicional deve ser maior que zero.");

                    f.RuleFor(x => x.value)
                        .Must(CustomerRules.IsAdditionalValue)
                        .WithMessage("O campo 'value' aceita texto até 80 caracteres, número com até 12 inteiros e 6 decimais ou data DD/MM/YYYY.")
                        .When(x => x.value is not null);
                })
                .When(x => x.additionalFields is not null);

            RuleFor(x => x.additionalData!)
                .ChildRules(a =>
                {
                    a.RuleFor(x => x.settleType).IsInEnum().WithMessage("O campo 'settleType' só aceita 0 (total e parcial) ou 1 (somente total).");

                    a.RuleFor(x => x)
                        .Must(x => new decimal?[] { x.currencyRateValue, x.maximumExtraQuantityPercentage, x.maximumExtraQuantity, x.accessoryExpensePercentage, x.freightPercentage,
                            x.insurancePercentage }.All(v => v is null or >= 0))
                        .WithName("additionalData")
                        .WithMessage("Os valores e percentuais de 'additionalData' não podem ser negativos.");
                })
                .When(x => x.additionalData is not null);
        }
    }
}
