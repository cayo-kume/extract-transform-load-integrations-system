using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script005_Create_Totvs_People_Tables)
    /// </summary>
    public class PreferencesValidator : AbstractValidator<Preferences>
    {
        public PreferencesValidator()
        {
            RuleFor(x => x.paymentConditionCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.paymentConditionCode));

            RuleFor(x => x.paymentConditionDescription)
                .MaximumLength(40)
                .WithMessage("O campo 'paymentConditionDescription' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.paymentConditionDescription));

            RuleFor(x => x.shippingCompanyCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyCode));

            RuleFor(x => x.shippingCompanyName)
                .MaximumLength(60)
                .WithMessage("O campo 'shippingCompanyName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.shippingCompanyName));

            RuleFor(x => x.freightType)
                .MaximumLength(30)
                .WithMessage("O campo 'freightType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.freightType));

            RuleFor(x => x.bridgeShippingCompanyCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.bridgeShippingCompanyCode));

            RuleFor(x => x.bridgeShippingCompanyName)
                .MaximumLength(60)
                .WithMessage("O campo 'bridgeShippingCompanyName' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bridgeShippingCompanyName));

            RuleFor(x => x.maximumAverageTerm)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.maximumAverageTerm));

            RuleFor(x => x.priceTableCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.priceTableCode));

            RuleFor(x => x.priceTableDescription)
                .MaximumLength(40)
                .WithMessage("O campo 'priceTableDescription' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.priceTableDescription));

            RuleFor(x => x.orderPriority)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.orderPriority));

            RuleFor(x => x.bearerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.bearerCode));

            RuleFor(x => x.bearerName)
                .MaximumLength(40)
                .WithMessage("O campo 'bearerName' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bearerName));
        }
    }
}
