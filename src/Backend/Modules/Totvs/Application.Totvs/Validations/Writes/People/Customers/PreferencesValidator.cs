using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// preferências comerciais da pessoa jurídica (PreferencesInDto): tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class PreferencesValidator : AbstractValidator<Preferences>
    {
        public PreferencesValidator()
        {
            RuleFor(x => x.paymentConditionCode)
                .InclusiveBetween(0, 9_999)
                .WithMessage("O campo 'paymentConditionCode' deve ter até 4 dígitos (0 zera).")
                .When(x => x.paymentConditionCode is not null);

            RuleFor(x => x.shippingCompanyCode)
                .InclusiveBetween(0, 999_999_999)
                .WithMessage("O campo 'shippingCompanyCode' deve ter até 9 dígitos (0 zera).")
                .When(x => x.shippingCompanyCode is not null);

            RuleFor(x => x.freightType)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.Freight))
                .WithMessage($"O campo 'freightType' só aceita {CustomerRules.Names(CustomerEnumNames.Freight)}.");

            RuleFor(x => x.bridgeShippingCompanyCode)
                .InclusiveBetween(0, 999_999_999)
                .WithMessage("O campo 'bridgeShippingCompanyCode' deve ter até 9 dígitos (0 zera).")
                .When(x => x.bridgeShippingCompanyCode is not null);

            RuleFor(x => x.maximumAverageTerm)
                .InclusiveBetween(0, 9_999)
                .WithMessage("O campo 'maximumAverageTerm' deve ter até 4 dígitos (0 zera).")
                .When(x => x.maximumAverageTerm is not null);

            RuleFor(x => x.priceTableCode)
                .InclusiveBetween(0, 999_999_999)
                .WithMessage("O campo 'priceTableCode' deve ter até 9 dígitos (0 zera).")
                .When(x => x.priceTableCode is not null);

            RuleFor(x => x.orderPriority)
                .InclusiveBetween(0, 99)
                .WithMessage("O campo 'orderPriority' deve ter até 2 dígitos (0 zera).")
                .When(x => x.orderPriority is not null);

            RuleFor(x => x.bearerCode)
                .InclusiveBetween(0, 9_999)
                .WithMessage("O campo 'bearerCode' deve ter até 4 dígitos (0 zera).")
                .When(x => x.bearerCode is not null);
        }
    }
}
