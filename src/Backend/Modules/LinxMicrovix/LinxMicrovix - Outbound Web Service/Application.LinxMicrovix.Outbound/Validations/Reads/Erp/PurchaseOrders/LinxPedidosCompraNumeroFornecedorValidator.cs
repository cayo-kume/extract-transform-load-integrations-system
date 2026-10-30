using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PurchaseOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.PurchaseOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script055_Create_LinxMicrovix_Purchase_Orders_Tables
    /// </summary>
    public class LinxPedidosCompraNumeroFornecedorValidator : AbstractValidator<LinxPedidosCompraNumeroFornecedor>
    {
        public LinxPedidosCompraNumeroFornecedorValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_pedido_fornecedor)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_pedido_fornecedor' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.numero_ped_erp)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_ped_erp));

            RuleFor(x => x.numero_ped_fornecedor)
                .MaximumLength(20)
                .WithMessage("O campo 'numero_ped_fornecedor' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_ped_fornecedor));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
