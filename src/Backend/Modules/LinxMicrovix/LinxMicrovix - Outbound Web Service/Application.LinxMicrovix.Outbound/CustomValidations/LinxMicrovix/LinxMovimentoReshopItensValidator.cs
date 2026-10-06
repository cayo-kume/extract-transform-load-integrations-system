using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoReshopItensValidator : AbstractValidator<LinxMovimentoReshopItens>
    {
        public LinxMovimentoReshopItensValidator()
        {
            RuleFor(x => x.id_movimento_campanha_reshop_item).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_movimento_campanha_reshop_item));
            RuleFor(x => x.id_campanha).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_campanha));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.valor_unitario).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_unitario));
            RuleFor(x => x.valor_desconto_item).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_desconto_item));
            RuleFor(x => x.quantidade).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.quantidade));
            RuleFor(x => x.valor_original).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_original));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.ordem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.ordem));
        }
    }
}