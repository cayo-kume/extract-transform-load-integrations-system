using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// item incluído: produto por código ou SKU, quantidade e preço (9 inteiros e 3 decimais), descontos, seriais e comissionados
    /// </summary>
    public class OrderItemAddValidator : AbstractValidator<OrderItemAdd>
    {
        public OrderItemAddValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.productCode is null) != String.IsNullOrEmpty(x.productSku))
                .WithName("productCode")
                .WithMessage("Informe exatamente um entre 'productCode' e 'productSku'.");

            RuleFor(x => x.productCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'productCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.productCode is not null);

            RuleFor(x => x.productSku)
                .MaximumLength(40)
                .WithMessage("O campo 'productSku' aceita até 40 caracteres.");

            RuleFor(x => x.quantity)
                .NotNull()
                .WithMessage("O campo 'quantity' é obrigatório.")
                .GreaterThan(0)
                .WithMessage("O campo 'quantity' deve ser maior que zero.")
                .LessThanOrEqualTo(999_999_999.999m)
                .WithMessage("O campo 'quantity' aceita até 9 inteiros.")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'quantity' aceita até 3 casas decimais.");

            RuleFor(x => x.price)
                .NotNull()
                .WithMessage("O campo 'price' é obrigatório.")
                .InclusiveBetween(0, 999_999_999.999m)
                .WithMessage("O campo 'price' aceita até 9 inteiros.")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'price' aceita até 3 casas decimais.");

            RuleFor(x => x.originalPrice)
                .InclusiveBetween(0, 999_999_999.999m)
                .WithMessage("O campo 'originalPrice' aceita até 9 inteiros.")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'originalPrice' aceita até 3 casas decimais.")
                .When(x => x.originalPrice is not null);

            RuleFor(x => x.discountPercentage)
                .InclusiveBetween(0, 100)
                .WithMessage("O campo 'discountPercentage' vai de 0 a 100.")
                .When(x => x.discountPercentage is not null);

            RuleFor(x => x.discountValue)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O campo 'discountValue' não pode ser negativo.")
                .When(x => x.discountValue is not null);

            RuleFor(x => x.customerProductCode)
                .MaximumLength(20)
                .WithMessage("O campo 'customerProductCode' aceita até 20 caracteres.");

            RuleFor(x => x.rollMeasureCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'rollMeasureCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.rollMeasureCode is not null);

            RuleFor(x => x.sequentials)
                .Must((x, l) => x.quantity is null || l!.Count == x.quantity)
                .WithMessage("A lista 'sequentials' tem de ter um serial por unidade de 'quantity'.")
                .When(x => x.sequentials is { Count: > 0 });

            RuleForEach(x => x.sequentialsValues)
                .ChildRules(s =>
                    s.RuleFor(x => x.code)
                        .MaximumLength(100)
                        .WithMessage("O campo 'code' do serial aceita até 100 caracteres."))
                .When(x => x.sequentialsValues is not null);

            RuleForEach(x => x.groupObservations)
                .ChildRules(o =>
                    o.RuleFor(x => x.observation)
                        .MaximumLength(80)
                        .WithMessage("A observação do grupo aceita até 80 caracteres."))
                .When(x => x.groupObservations is not null);

            RuleForEach(x => x.commissioneds)
                .ChildRules(c =>
                {
                    c.RuleFor(x => x)
                        .Must(x => (x.representativeCode is null) != String.IsNullOrEmpty(x.representativeCpfCnpj))
                        .WithName("representativeCode")
                        .WithMessage("Informe exatamente um entre 'representativeCode' e 'representativeCpfCnpj' no comissionado.");

                    c.RuleFor(x => x.representativeCpfCnpj)
                        .Matches(@"^(\d{11}|\d{14})$")
                        .WithMessage("O campo 'representativeCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                        .When(x => !String.IsNullOrEmpty(x.representativeCpfCnpj));

                    c.RuleFor(x => x.commissionedType)
                        .NotNull()
                        .WithMessage("O campo 'commissionedType' é obrigatório (1 representante, 2 supervisor, 3 corretor, 90 geral).")
                        .IsInEnum()
                        .WithMessage("O campo 'commissionedType' só aceita 1, 2, 3 ou 90.");

                    c.RuleFor(x => x)
                        .Must(x => x.commissionPercentageBilling is null or >= 0 and <= 100 && x.commissionPercentageReceipt is null or >= 0 and <= 100)
                        .WithName("commissionPercentageBilling")
                        .WithMessage("As comissões do comissionado vão de 0 a 100.");
                })
                .When(x => x.commissioneds is not null);
        }
    }
}
