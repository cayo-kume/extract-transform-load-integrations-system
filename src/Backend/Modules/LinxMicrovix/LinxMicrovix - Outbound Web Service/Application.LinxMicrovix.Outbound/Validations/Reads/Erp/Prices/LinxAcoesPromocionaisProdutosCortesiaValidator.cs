using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Prices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script025_Create_LinxMicrovix_Prices_Tables
    /// </summary>
    public class LinxAcoesPromocionaisProdutosCortesiaValidator : AbstractValidator<LinxAcoesPromocionaisProdutosCortesia>
    {
        public LinxAcoesPromocionaisProdutosCortesiaValidator()
        {
            RuleFor(x => x.id_acoes_promocionais_produtos_cortesia)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_acoes_promocionais_produtos_cortesia' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_acoes_promocionais)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_acoes_promocionais));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.id_combinacao_produto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_combinacao_produto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
