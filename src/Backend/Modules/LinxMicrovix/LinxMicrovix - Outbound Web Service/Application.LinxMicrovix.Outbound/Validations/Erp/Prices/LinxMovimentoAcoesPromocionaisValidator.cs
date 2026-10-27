using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Prices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script025_Create_LinxMicrovix_Prices_Tables
    /// </summary>
    public class LinxMovimentoAcoesPromocionaisValidator : AbstractValidator<LinxMovimentoAcoesPromocionais>
    {
        public LinxMovimentoAcoesPromocionaisValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.transacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao));

            RuleFor(x => x.id_acoes_promocionais)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_acoes_promocionais));

            RuleFor(x => x.desconto_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto_item));

            RuleFor(x => x.quantidade)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.quantidade));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
