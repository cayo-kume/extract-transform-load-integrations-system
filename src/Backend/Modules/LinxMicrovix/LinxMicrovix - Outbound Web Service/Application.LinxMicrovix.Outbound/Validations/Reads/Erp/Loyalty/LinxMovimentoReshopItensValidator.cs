using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Loyalty;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Loyalty
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script037_Create_LinxMicrovix_Loyalty_Tables
    /// </summary>
    public class LinxMovimentoReshopItensValidator : AbstractValidator<LinxMovimentoReshopItens>
    {
        public LinxMovimentoReshopItensValidator()
        {
            RuleFor(x => x.id_movimento_campanha_reshop_item)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_movimento_campanha_reshop_item' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_campanha)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_campanha));

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.valor_unitario)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_unitario));

            RuleFor(x => x.valor_desconto_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_desconto_item));

            RuleFor(x => x.valor_total_desconto_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_total_desconto_item));

            RuleFor(x => x.quantidade)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantidade));

            RuleFor(x => x.valor_original)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_original));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.ordem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ordem));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
