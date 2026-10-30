using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Sellers;
using FluentValidation;

namespace Application.Totvs.Validations.Sellers
{
    /// <summary>
    /// só registra mensagens no log; o swagger da API Seller não dá tamanho dos textos (colunas do
    /// Script008_Create_Totvs_Sellers_Tables)
    /// </summary>
    public class OperationalAreaValidator : AbstractValidator<OperationalArea>
    {
        public OperationalAreaValidator()
        {
            RuleFor(x => x.operationalAreaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'operationalAreaCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.operationalAreaName)
                .MaximumLength(100)
                .WithMessage("O campo 'operationalAreaName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operationalAreaName));

            RuleFor(x => x.operationalAreaType)
                .MaximumLength(30)
                .WithMessage("O campo 'operationalAreaType' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operationalAreaType));

            RuleFor(x => x.operationalAreaGroupCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.operationalAreaGroupCode));

            RuleFor(x => x.operationalAreaGroupDescription)
                .MaximumLength(100)
                .WithMessage("O campo 'operationalAreaGroupDescription' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operationalAreaGroupDescription));

            RuleFor(x => x.numberOfDays)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numberOfDays));

            RuleFor(x => x.operationalAreaType)
                .Must(x => x is "Representative" or "ShippingCompany" or "DeliveryDate" or "CreditLocation")
                .WithMessage("O campo 'operationalAreaType' só aceita Representative, ShippingCompany, DeliveryDate ou CreditLocation.")
                .When(x => !string.IsNullOrEmpty(x.operationalAreaType));
        }
    }
}
