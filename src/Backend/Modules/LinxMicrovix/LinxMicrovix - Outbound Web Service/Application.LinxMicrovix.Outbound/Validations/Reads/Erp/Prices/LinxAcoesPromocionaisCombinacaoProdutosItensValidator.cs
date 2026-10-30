using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Prices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script025_Create_LinxMicrovix_Prices_Tables
    /// </summary>
    public class LinxAcoesPromocionaisCombinacaoProdutosItensValidator : AbstractValidator<LinxAcoesPromocionaisCombinacaoProdutosItens>
    {
        public LinxAcoesPromocionaisCombinacaoProdutosItensValidator()
        {
            RuleFor(x => x.id_acoes_promocionais_combinacao_produtos_itens)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_acoes_promocionais_combinacao_produtos_itens' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_combinacao_produto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_combinacao_produto));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
