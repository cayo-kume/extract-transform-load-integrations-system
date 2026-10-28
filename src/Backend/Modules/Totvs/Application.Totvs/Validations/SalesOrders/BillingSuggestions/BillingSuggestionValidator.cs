using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders.BillingSuggestions;
using FluentValidation;

namespace Application.Totvs.Validations.SalesOrders.BillingSuggestions
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order e das colunas do Script057_Create_Totvs_Sales_Order_Catalogs_Tables (textos dos itens: os tamanhos de TotvsSaleOrdersProducts)
    /// </summary>
    public class BillingSuggestionValidator : AbstractValidator<BillingSuggestion>
    {
        public BillingSuggestionValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.suggestionCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'suggestionCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.statusSuggestion)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.statusSuggestion));

            RuleFor(x => x.suggestionDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.suggestionDate));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

            RuleForEach(x => x.orders)
                .Must(x => !string.IsNullOrEmpty(x.orderBranchCode) && !string.IsNullOrEmpty(x.orderCode))
                .WithMessage("Pedido da sugestão sem 'orderBranchCode' ou 'orderCode': descartado.");
        }
    }
}
