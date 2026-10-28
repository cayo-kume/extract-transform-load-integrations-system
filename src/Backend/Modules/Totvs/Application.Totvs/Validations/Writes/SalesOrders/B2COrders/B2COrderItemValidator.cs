using Domain.Totvs.Dtos.Writes.SalesOrders.B2COrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders.B2COrders
{
    /// <summary>
    /// item do pedido B2C: produto por código ou SKU, quantidade e preço (9 inteiros e 3 decimais), seriais um por unidade, terceiros com a
    /// quantidade distribuída e comissionados com o tipo
    /// </summary>
    public class B2COrderItemValidator : AbstractValidator<B2COrderItem>
    {
        public B2COrderItemValidator()
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
                .WithMessage("O campo 'quantity' do item é obrigatório.")
                .GreaterThan(0)
                .WithMessage("O campo 'quantity' do item deve ser maior que zero.")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'quantity' do item aceita até 3 casas decimais.");

            RuleFor(x => x.price)
                .NotNull()
                .WithMessage("O campo 'price' do item é obrigatório.")
                .InclusiveBetween(0, 999_999_999.999m)
                .WithMessage("O campo 'price' do item aceita até 9 inteiros.")
                .PrecisionScale(12, 3, true)
                .WithMessage("O campo 'price' do item aceita até 3 casas decimais.");

            RuleFor(x => x.discountPercentage)
                .InclusiveBetween(0, 100)
                .WithMessage("O campo 'discountPercentage' do item vai de 0 a 100.")
                .When(x => x.discountPercentage is not null);

            RuleFor(x => x.customerProductCode).MaximumLength(20).WithMessage("O campo 'customerProductCode' aceita até 20 caracteres.");

            RuleFor(x => x)
                .Must(x => x.quantity is null || ((x.sequentials is not { Count: > 0 } || x.sequentials.Count == x.quantity) && (x.sequentialsValues is not { Count: > 0 } || x.sequentialsValues.Count == x.quantity)))
                .WithName("sequentials")
                .WithMessage("'sequentials' e 'sequentialsValues' têm um código por unidade de 'quantity'.");

            RuleForEach(x => x.outsourceds)
                .ChildRules(o =>
                {
                    o.RuleFor(x => x.cep).NotEmpty().WithMessage("O campo 'cep' do terceiro do item é obrigatório.");
                    o.RuleFor(x => x).Must(x => (x.customerCode is null) != String.IsNullOrEmpty(x.customerCpfCnpj)).WithName("customerCode")
                        .WithMessage("Informe exatamente um entre 'customerCode' e 'customerCpfCnpj' no terceiro do item.");
                    o.RuleFor(x => x.distributionQuantity).NotNull().WithMessage("O campo 'distributionQuantity' é obrigatório.");
                })
                .When(x => x.outsourceds is not null);

            RuleForEach(x => x.commissioneds)
                .ChildRules(c =>
                {
                    c.RuleFor(x => x)
                        .Must(x => (x.representativeCode is null) != String.IsNullOrEmpty(x.representativeCpfCnpj))
                        .WithName("representativeCode")
                        .WithMessage("Informe exatamente um entre 'representativeCode' e 'representativeCpfCnpj' no comissionado do item.");

                    c.RuleFor(x => x.commissionedType).NotNull().WithMessage("O campo 'commissionedType' do item é obrigatório.").IsInEnum().WithMessage("O campo 'commissionedType' só aceita 1, 2, 3 ou 90.");
                })
                .When(x => x.commissioneds is not null);

            RuleForEach(x => x.groupObservations)
                .ChildRules(o => o.RuleFor(x => x.observation).MaximumLength(80).WithMessage("A observação do grupo aceita até 80 caracteres."))
                .When(x => x.groupObservations is not null);
        }
    }
}
