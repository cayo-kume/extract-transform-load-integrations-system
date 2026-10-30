using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PurchaseOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.PurchaseOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script055_Create_LinxMicrovix_Purchase_Orders_Tables
    /// </summary>
    public class LinxPedidosCompraStatusValidator : AbstractValidator<LinxPedidosCompraStatus>
    {
        public LinxPedidosCompraStatusValidator()
        {
            RuleFor(x => x.id_compras_pedido_status)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_compras_pedido_status' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(50)
                .WithMessage("O campo 'descricao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
