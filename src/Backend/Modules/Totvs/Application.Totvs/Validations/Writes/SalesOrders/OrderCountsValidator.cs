using Domain.Totvs.Dtos.Writes.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.SalesOrders
{
    /// <summary>
    /// valida o corpo do vínculo de contagens no controller (corpo inválido volta 400): loja e número do pedido e pelo menos uma contagem, com a
    /// sugestão aos pares (o schema não marca nada como obrigatório)
    /// </summary>
    public class OrderCountsValidator : AbstractValidator<OrderCounts>
    {
        public OrderCountsValidator()
        {
            RuleFor(x => x.branchCode)
                .NotNull()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .InclusiveBetween(1, 9_999)
                .WithMessage("O campo 'branchCode' deve ter de 1 a 4 dígitos.");

            RuleFor(x => x.orderCode)
                .NotNull()
                .WithMessage("O campo 'orderCode' é obrigatório.")
                .InclusiveBetween(1, 999_999_999)
                .WithMessage("O campo 'orderCode' deve ter de 1 a 9 dígitos.");

            RuleFor(x => x.counts)
                .NotEmpty()
                .WithMessage("Informe pelo menos uma contagem em 'counts'.");

            RuleForEach(x => x.counts)
                .ChildRules(c =>
                {
                    c.RuleFor(x => x.branchCountCode)
                        .NotNull()
                        .WithMessage("O campo 'branchCountCode' é obrigatório.")
                        .InclusiveBetween(1, 9_999)
                        .WithMessage("O campo 'branchCountCode' deve ter de 1 a 4 dígitos.");

                    c.RuleFor(x => x.countCode)
                        .NotNull()
                        .WithMessage("O campo 'countCode' é obrigatório.")
                        .InclusiveBetween(1, 999_999_999)
                        .WithMessage("O campo 'countCode' deve ter de 1 a 9 dígitos.");

                    c.RuleFor(x => x)
                        .Must(x => (x.branchSuggestionCode is null) == (x.suggestionCode is null))
                        .WithName("branchSuggestionCode")
                        .WithMessage("A sugestão vai com 'branchSuggestionCode' e 'suggestionCode', os dois.");
                })
                .When(x => x.counts is not null);
        }
    }
}
