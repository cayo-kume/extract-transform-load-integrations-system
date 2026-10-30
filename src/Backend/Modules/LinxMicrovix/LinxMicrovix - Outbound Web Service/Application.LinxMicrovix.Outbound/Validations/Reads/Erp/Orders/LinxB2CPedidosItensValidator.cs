using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Orders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script052_Create_LinxMicrovix_Orders_Tables
    /// </summary>
    public class LinxB2CPedidosItensValidator : AbstractValidator<LinxB2CPedidosItens>
    {
        public LinxB2CPedidosItensValidator()
        {
            RuleFor(x => x.id_pedido_item)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_pedido_item' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_pedido)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_pedido));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.quantidade)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.quantidade));

            RuleFor(x => x.vl_unitario)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.vl_unitario));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
