using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo do transporte do pedido no controller (corpo inválido volta 400): a chave, pelo menos um campo e as regras de
    /// transportadora e redespacho da descrição do swagger (ManageOrderShippingCommand)
    /// </summary>
    public class OrderShippingValidator : AbstractValidator<OrderShipping>
    {
        public OrderShippingValidator()
        {
            Include(new OrderKeyValidator());

            RuleFor(x => x)
                .Must(x => x.shippingCompanyCode is not null || x.shippingCompanyCpfCnpj is not null || x.redispatchingShippingCompanyCode is not null
                    || x.redispatchingShippingCompanyCpfCnpj is not null || x.redispatchingFreightType is not null || x.freightType is not null
                    || x.freightPercentage is not null || x.freightValue is not null || x.shippingAddress is not null)
                .WithName("shippingCompanyCode")
                .WithMessage("Informe pelo menos um campo além da chave do pedido.");

            RuleFor(x => x)
                .Must(x => x.shippingCompanyCode is null || String.IsNullOrEmpty(x.shippingCompanyCpfCnpj))
                .WithName("shippingCompanyCode")
                .WithMessage("Informe a transportadora por 'shippingCompanyCode' ou por 'shippingCompanyCpfCnpj', não os dois.");

            RuleFor(x => x.freightType)
                .NotNull()
                .WithMessage("Com a transportadora, o campo 'freightType' é obrigatório.")
                .When(x => x.shippingCompanyCode is not null || !String.IsNullOrEmpty(x.shippingCompanyCpfCnpj));

            RuleFor(x => x.freightType)
                .IsInEnum()
                .WithMessage("O campo 'freightType' só aceita de 1 a 6.");

            RuleFor(x => x)
                .Must(x => x.redispatchingShippingCompanyCode is null || String.IsNullOrEmpty(x.redispatchingShippingCompanyCpfCnpj))
                .WithName("redispatchingShippingCompanyCode")
                .WithMessage("Informe o redespacho por 'redispatchingShippingCompanyCode' ou por 'redispatchingShippingCompanyCpfCnpj', não os dois.");

            RuleFor(x => x.redispatchingFreightType)
                .NotNull()
                .WithMessage("Com o redespacho, o campo 'redispatchingFreightType' é obrigatório.")
                .When(x => x.redispatchingShippingCompanyCode is not null || !String.IsNullOrEmpty(x.redispatchingShippingCompanyCpfCnpj));

            RuleFor(x => x.redispatchingFreightType)
                .Must(x => x is 1 or 2)
                .WithMessage("O campo 'redispatchingFreightType' só aceita 1 (emitente) ou 2 (destinatário).")
                .Must((x, _) => x.redispatchingShippingCompanyCode is not null || !String.IsNullOrEmpty(x.redispatchingShippingCompanyCpfCnpj))
                .WithMessage("O campo 'redispatchingFreightType' só vai junto com a transportadora de redespacho.")
                .When(x => x.redispatchingFreightType is not null);

            RuleFor(x => x.shippingCompanyCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'shippingCompanyCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.shippingCompanyCode is not null);

            RuleFor(x => x.redispatchingShippingCompanyCode)
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'redispatchingShippingCompanyCode' deve ter de 1 a 9 dígitos.")
                .When(x => x.redispatchingShippingCompanyCode is not null);

            RuleFor(x => x.shippingCompanyCpfCnpj)
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'shippingCompanyCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.shippingCompanyCpfCnpj));

            RuleFor(x => x.redispatchingShippingCompanyCpfCnpj)
                .Matches(@"^(\d{11}|\d{14})$")
                .WithMessage("O campo 'redispatchingShippingCompanyCpfCnpj' deve ter 11 (CPF) ou 14 (CNPJ) dígitos, sem pontuação.")
                .When(x => !String.IsNullOrEmpty(x.redispatchingShippingCompanyCpfCnpj));

            RuleFor(x => x.freightPercentage)
                .InclusiveBetween(0, 999.999999m)
                .WithMessage("O campo 'freightPercentage' deve ficar entre 0 e 999,999999.")
                .PrecisionScale(9, 6, true)
                .WithMessage("O campo 'freightPercentage' aceita até 6 casas decimais.")
                .When(x => x.freightPercentage is not null);

            RuleFor(x => x.freightValue)
                .InclusiveBetween(0, 999_999_999_999.99m)
                .WithMessage("O campo 'freightValue' deve ficar entre 0 e 999.999.999.999,99.")
                .PrecisionScale(14, 2, true)
                .WithMessage("O campo 'freightValue' aceita até 2 casas decimais.")
                .When(x => x.freightValue is not null);

            RuleFor(x => x.shippingAddress!)
                .ChildRules(address =>
                {
                    address.RuleFor(a => a.cep).Matches(@"^\d{8}$").WithMessage("O campo 'shippingAddress.cep' deve ter 8 dígitos, sem pontuação.").When(a => a.cep is not null);
                    address.RuleFor(a => a.publicPlace).MaximumLength(10).WithMessage("O campo 'shippingAddress.publicPlace' aceita até 10 caracteres.");
                    address.RuleFor(a => a.address).MaximumLength(60).WithMessage("O campo 'shippingAddress.address' aceita até 60 caracteres.");
                    address.RuleFor(a => a.number).InclusiveBetween(0, 99_999).WithMessage("O campo 'shippingAddress.number' deve ter até 5 dígitos.").When(a => a.number is not null);
                    address.RuleFor(a => a.complement).MaximumLength(65).WithMessage("O campo 'shippingAddress.complement' aceita até 65 caracteres.");
                })
                .When(x => x.shippingAddress is not null);
        }
    }
}
