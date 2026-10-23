using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script013_Create_Totvs_People_Tables)
    /// </summary>
    public class ShippingCompanyValidator : AbstractValidator<ShippingCompany>
    {
        public ShippingCompanyValidator()
        {
            RuleFor(x => x.percentegeValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentegeValue));

            RuleFor(x => x.kilogramsValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.kilogramsValue));

            RuleFor(x => x.minimunValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.minimunValue));

            RuleFor(x => x.maximumValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.maximumValue));

            RuleFor(x => x.anttCode)
                .MaximumLength(80)
                .WithMessage("O campo 'anttCode' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.anttCode));

            RuleFor(x => x.typeDeductIcms)
                .MaximumLength(20)
                .WithMessage("O campo 'typeDeductIcms' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeDeductIcms));
        }
    }
}
