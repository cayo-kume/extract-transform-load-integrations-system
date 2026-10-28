using Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// valida o corpo do pedido B2C no controller (corpo inválido volta 400). Obrigatórios pela descrição do swagger (o schema marca os dois
    /// lados de cada "um ou outro"): cliente e representante por código ou CPF/CNPJ, exatamente um; transportadora e redespacho com o tipo de
    /// frete; soma dos pagamentos igual ao total (0,01 de tolerância); integrationCode só com algum paymentType. A soma dos itens fica com a
    /// Totvs (descontos de capa e de item, frete e arredondamento)
    /// </summary>
    public class B2COrderValidator : AbstractValidator<B2COrder>
    {
        public B2COrderValidator()
        {
            // cada par código ou CPF/CNPJ: no máximo um; os obrigatórios têm regra própria
            static bool AtMostOne(int? code, string? document) => code is null || String.IsNullOrEmpty(document);
            static bool IsDocument(string? document) => String.IsNullOrEmpty(document) || System.Text.RegularExpressions.Regex.IsMatch(document, @"^(\d{11}|\d{14})$");

            RuleFor(x => x.orderId)
                .NotEmpty()
                .WithMessage("O campo 'orderId' é obrigatório (é o controle de duplicidade).")
                .MaximumLength(100)
                .WithMessage("O campo 'orderId' aceita até 100 caracteres.");

            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.orderDate).NotNull().WithMessage("O campo 'orderDate' é obrigatório.");

            RuleFor(x => x)
                .Must(x => (x.customerCode is null) != String.IsNullOrEmpty(x.customerCpfCnpj))
                .WithName("customerCode")
                .WithMessage("Informe exatamente um entre 'customerCode' e 'customerCpfCnpj'.");

            RuleFor(x => x)
                .Must(x => (x.representativeCode is null) != String.IsNullOrEmpty(x.representativeCpfCnpj))
                .WithName("representativeCode")
                .WithMessage("Informe exatamente um entre 'representativeCode' e 'representativeCpfCnpj'.");

            RuleFor(x => x)
                .Must(x => AtMostOne(x.intermediaryCode, x.intermediaryCnpj) && AtMostOne(x.guideCode, x.guideCpfCnpj) && AtMostOne(x.sellerCode, x.sellerCpfCnpj)
                    && AtMostOne(x.shippingCompanyCode, x.shippingCompanyCpfCnpj) && AtMostOne(x.redispatchingShippingCompanyCode, x.redispatchingShippingCompanyCpfCnpj)
                    && AtMostOne(x.buyerCustomerCode, x.buyerCustomerCpfCnpj))
                .WithName("intermediaryCode")
                .WithMessage("Intermediário, guia, vendedor, transportadora, redespacho e comprador vão pelo código ou pelo CPF/CNPJ, não os dois.");

            RuleFor(x => x)
                .Must(x => new[] { x.customerCpfCnpj, x.representativeCpfCnpj, x.intermediaryCnpj, x.guideCpfCnpj, x.sellerCpfCnpj, x.shippingCompanyCpfCnpj,
                    x.redispatchingShippingCompanyCpfCnpj, x.buyerCustomerCpfCnpj }.All(IsDocument))
                .WithName("customerCpfCnpj")
                .WithMessage("Os CPF/CNPJ do pedido têm 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.");

            RuleFor(x => x.sellerCode)
                .InclusiveBetween(1, 999_999)
                .WithMessage("O campo 'sellerCode' deve ter até 6 dígitos.")
                .When(x => x.sellerCode is not null);

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

            RuleFor(x => x.priorityCode)
                .NotNull()
                .WithMessage("O campo 'priorityCode' é obrigatório.")
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'priorityCode' deve ter até 2 dígitos.");

            RuleFor(x => x.statusOrder)
                .NotNull()
                .WithMessage("O campo 'statusOrder' é obrigatório (1 em andamento, 5 bloqueado, 8 em análise).")
                .IsInEnum()
                .WithMessage("O campo 'statusOrder' só aceita 1, 5 ou 8.");

            RuleFor(x => x.totalAmountOrder)
                .NotNull()
                .WithMessage("O campo 'totalAmountOrder' é obrigatório.")
                .InclusiveBetween(0, 999_999_999_999.99m)
                .WithMessage("O campo 'totalAmountOrder' aceita até 12 inteiros.")
                .PrecisionScale(14, 2, true)
                .WithMessage("O campo 'totalAmountOrder' aceita até 2 casas decimais.");

            RuleFor(x => x)
                .Must(x => (x.shippingCompanyCode is null && String.IsNullOrEmpty(x.shippingCompanyCpfCnpj)) || x.freightType is not null)
                .WithName("freightType")
                .WithMessage("Com transportadora, o 'freightType' é obrigatório.");

            RuleFor(x => x)
                .Must(x => (x.redispatchingShippingCompanyCode is null && String.IsNullOrEmpty(x.redispatchingShippingCompanyCpfCnpj)) == (x.redispatchingFreightType is null))
                .WithName("redispatchingFreightType")
                .WithMessage("O redespacho vai com a transportadora e o 'redispatchingFreightType' (1 emitente, 2 destinatário), os dois.");

            RuleFor(x => x.redispatchingFreightType)
                .InclusiveBetween(1, 2)
                .WithMessage("O campo 'redispatchingFreightType' só aceita 1 (emitente) ou 2 (destinatário).")
                .When(x => x.redispatchingFreightType is not null);

            RuleFor(x => x.freightType).IsInEnum().WithMessage("O campo 'freightType' só aceita de 1 a 6.");
            RuleFor(x => x.chargeType).IsInEnum().WithMessage("O campo 'chargeType' só aceita 0, 16, 17, 18 ou 28.");

            RuleFor(x => x.experienceType)
                .Must(e => e is "Ecommerce" or "ShipFromStore" or "ClickAndCollect" or "PickupStore" or "InfiniteShelf")
                .WithMessage("O campo 'experienceType' só aceita Ecommerce, ShipFromStore, ClickAndCollect, PickupStore ou InfiniteShelf (pelo nome).")
                .When(x => !String.IsNullOrEmpty(x.experienceType));

            RuleFor(x => x.reasonBlockingCode)
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'reasonBlockingCode' deve ter de 1 a 4 dígitos.")
                .When(x => x.reasonBlockingCode is not null);

            RuleFor(x => x)
                .Must(x => new decimal?[] { x.freightValue, x.freightPercentage, x.packageNumber, x.weight, x.additionalValue }.All(v => v is null or >= 0))
                .WithName("freightValue")
                .WithMessage("Frete, volumes, peso e valor adicional não podem ser negativos.");

            RuleFor(x => x.customerOrderCode).MaximumLength(100).WithMessage("O campo 'customerOrderCode' aceita até 100 caracteres.");
            RuleFor(x => x.integrationCode).MaximumLength(100).WithMessage("O campo 'integrationCode' aceita até 100 caracteres.");
            RuleFor(x => x.shippingService).MaximumLength(10).WithMessage("O campo 'shippingService' aceita até 10 caracteres.");
            RuleFor(x => x.batchOrder).MaximumLength(10).WithMessage("O campo 'batchOrder' aceita até 10 caracteres.");
            RuleFor(x => x.receivingPerson).MaximumLength(100).WithMessage("O campo 'receivingPerson' aceita até 100 caracteres.");

            RuleFor(x => x.items)
                .NotEmpty()
                .WithMessage("Informe pelo menos um item em 'items'.");

            RuleForEach(x => x.items)
                .SetValidator(new B2COrderItemValidator())
                .When(x => x.items is not null);

            RuleForEach(x => x.payments)
                .SetValidator(new B2COrderPaymentValidator())
                .When(x => x.payments is not null);

            RuleFor(x => x)
                .Must(x => Math.Abs(x.payments!.Sum(p => p?.paymentValue ?? 0) - x.totalAmountOrder!.Value) <= 0.01m)
                .WithName("payments")
                .WithMessage("A soma dos 'paymentValue' precisa fechar com o 'totalAmountOrder' (0,01 de tolerância).")
                .When(x => x.payments is { Count: > 0 } && x.totalAmountOrder is not null);

            RuleFor(x => x.integrationCode)
                .Empty()
                .WithMessage("O campo 'integrationCode' só vai em split de pedido, com algum 'payments.paymentType'.")
                .When(x => x.payments is null || x.payments.All(p => p?.paymentType is null));

            RuleForEach(x => x.discounts)
                .ChildRules(dd =>
                {
                    dd.RuleFor(x => x.typeDiscountCode)
                        .NotNull()
                        .WithMessage("O campo 'typeDiscountCode' é obrigatório.")
                        .InclusiveBetween(1, 999)
                        .WithMessage("O campo 'typeDiscountCode' deve ter de 1 a 3 dígitos.");

                    dd.RuleFor(x => x)
                        .Must(x => (x.discountValue is null) != (x.discountPercentage is null))
                        .WithName("discountValue")
                        .WithMessage("Informe exatamente um entre 'discountValue' e 'discountPercentage' no desconto.");
                })
                .When(x => x.discounts is not null);

            RuleForEach(x => x.classifications)
                .ChildRules(c =>
                {
                    c.RuleFor(x => x.classificationTypeCode).NotNull().WithMessage("O campo 'classificationTypeCode' é obrigatório.");
                    c.RuleFor(x => x.classificationCode)
                        .NotEmpty()
                        .WithMessage("O campo 'classificationCode' é obrigatório.")
                        .MaximumLength(10)
                        .WithMessage("O campo 'classificationCode' aceita até 10 caracteres.");
                })
                .When(x => x.classifications is not null);

            RuleForEach(x => x.observations)
                .ChildRules(o =>
                {
                    o.RuleFor(x => x.observation).NotEmpty().WithMessage("O campo 'observation' é obrigatório.").MaximumLength(80).WithMessage("O campo 'observation' aceita até 80 caracteres.");
                    o.RuleFor(x => x.visualizationType).IsInEnum().WithMessage("O campo 'visualizationType' só aceita 1 ou 2.");
                })
                .When(x => x.observations is not null);

            RuleForEach(x => x.invoiceObservations)
                .ChildRules(o => o.RuleFor(x => x.observation).NotEmpty().WithMessage("O campo 'observation' é obrigatório.").MaximumLength(80).WithMessage("O campo 'observation' aceita até 80 caracteres."))
                .When(x => x.invoiceObservations is not null);

            RuleForEach(x => x.representativeObservations)
                .ChildRules(o => o.RuleFor(x => x.observation).NotEmpty().WithMessage("O campo 'observation' é obrigatório.").MaximumLength(80).WithMessage("O campo 'observation' aceita até 80 caracteres."))
                .When(x => x.representativeObservations is not null);

            RuleFor(x => x.shippingAddress!.cep)
                .NotEmpty()
                .WithMessage("O campo 'shippingAddress.cep' é obrigatório.")
                .Matches(@"^\d{8}$")
                .WithMessage("O campo 'shippingAddress.cep' deve ter 8 dígitos.")
                .When(x => x.shippingAddress is not null);

            RuleForEach(x => x.gifts)
                .ChildRules(g =>
                {
                    g.RuleFor(x => x)
                        .Must(x => (x.productCode is null) != String.IsNullOrEmpty(x.productSku))
                        .WithName("productCode")
                        .WithMessage("Informe exatamente um entre 'productCode' e 'productSku' no brinde.");

                    g.RuleFor(x => x.quantity).NotNull().WithMessage("O campo 'quantity' do brinde é obrigatório.").GreaterThan(0).WithMessage("O campo 'quantity' do brinde deve ser maior que zero.");
                    g.RuleFor(x => x.price).NotNull().WithMessage("O campo 'price' do brinde é obrigatório.").GreaterThanOrEqualTo(0).WithMessage("O campo 'price' do brinde não pode ser negativo.");
                })
                .When(x => x.gifts is not null);

            RuleForEach(x => x.commissioneds)
                .ChildRules(c =>
                {
                    c.RuleFor(x => x)
                        .Must(x => (x.representativeCode is null) != String.IsNullOrEmpty(x.representativeCpfCnpj))
                        .WithName("representativeCode")
                        .WithMessage("Informe exatamente um entre 'representativeCode' e 'representativeCpfCnpj' no comissionado.");

                    c.RuleFor(x => x.commissionedType).NotNull().WithMessage("O campo 'commissionedType' é obrigatório.").IsInEnum().WithMessage("O campo 'commissionedType' só aceita 1, 2, 3 ou 90.");
                })
                .When(x => x.commissioneds is not null);

            RuleForEach(x => x.outsourceds)
                .ChildRules(o =>
                {
                    o.RuleFor(x => x.cep).NotEmpty().WithMessage("O campo 'cep' do terceiro é obrigatório.");
                    o.RuleFor(x => x).Must(x => (x.customerCode is null) != String.IsNullOrEmpty(x.customerCpfCnpj)).WithName("customerCode")
                        .WithMessage("Informe exatamente um entre 'customerCode' e 'customerCpfCnpj' no terceiro.");
                    o.RuleFor(x => x.operationCode).NotNull().WithMessage("O campo 'operationCode' do terceiro é obrigatório.");
                })
                .When(x => x.outsourceds is not null);

            RuleForEach(x => x.billingPercentageLimitDates)
                .ChildRules(b => b.RuleFor(x => x.limitType).IsInEnum().WithMessage("O campo 'limitType' só aceita 1 ou 2."))
                .When(x => x.billingPercentageLimitDates is not null);
        }
    }
}
