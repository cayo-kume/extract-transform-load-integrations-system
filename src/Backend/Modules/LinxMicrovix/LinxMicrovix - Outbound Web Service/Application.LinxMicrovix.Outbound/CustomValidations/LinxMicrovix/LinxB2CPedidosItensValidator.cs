using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxB2CPedidosItensValidator : AbstractValidator<LinxB2CPedidosItens>
    {
        public LinxB2CPedidosItensValidator()
        {
            RuleFor(x => x.id_pedido_item).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_pedido_item));
            RuleFor(x => x.id_pedido).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_pedido));
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.quantidade).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.quantidade));
            RuleFor(x => x.vl_unitario).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.vl_unitario));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
