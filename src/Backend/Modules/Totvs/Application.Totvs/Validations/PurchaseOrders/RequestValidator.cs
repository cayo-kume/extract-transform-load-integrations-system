using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.PurchaseOrders;
using FluentValidation;

namespace Application.Totvs.Validations.PurchaseOrders
{
    /// <summary>
    /// só registra mensagens no log; o swagger da API Purchase Order não traz tamanhos: os do código antigo, conferidos numa amostra real (colunas do script de tabelas de pedido de compras)
    /// </summary>
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.requestCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'requestCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.requestDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.requestDate));
        }
    }
}
