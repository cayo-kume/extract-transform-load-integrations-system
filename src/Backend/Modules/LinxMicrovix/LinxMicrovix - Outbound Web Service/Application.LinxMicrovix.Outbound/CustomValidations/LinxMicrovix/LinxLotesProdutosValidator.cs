using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxLotesProdutosValidator : AbstractValidator<LinxLotesProdutos>
    {
        public LinxLotesProdutosValidator()
        {
            RuleFor(x => x.id_lote).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_lote));
            RuleFor(x => x.codigo_produto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigo_produto));
            RuleFor(x => x.lote).MaximumLength(60).WithMessage("O campo 'lote' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.lote));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.transacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao));
            RuleFor(x => x.data_fabricacao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_fabricacao));
            RuleFor(x => x.data_vencimento).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_vencimento));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
