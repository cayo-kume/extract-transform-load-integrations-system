using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxCupomDescontoVendasValidator : AbstractValidator<LinxCupomDescontoVendas>
    {
        public LinxCupomDescontoVendasValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.id_cupom_desconto_vendas).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_cupom_desconto_vendas));
            RuleFor(x => x.id_cupom_desconto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_cupom_desconto));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.valor).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.id_vendas_pos).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_vendas_pos));
        }
    }
}
