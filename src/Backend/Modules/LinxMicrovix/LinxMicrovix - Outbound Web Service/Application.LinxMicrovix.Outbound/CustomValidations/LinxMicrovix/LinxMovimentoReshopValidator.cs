using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoReshopValidator : AbstractValidator<LinxMovimentoReshop>
    {
        public LinxMovimentoReshopValidator()
        {
            RuleFor(x => x.id_movimento_campanha_reshop).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_movimento_campanha_reshop));
            RuleFor(x => x.id_campanha).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_campanha));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.nome).MaximumLength(200).WithMessage("O campo 'nome' deve ter no máximo 200 caracteres.").When(x => !string.IsNullOrEmpty(x.nome));
            RuleFor(x => x.descricao).MaximumLength(200).WithMessage("O campo 'descricao' deve ter no máximo 200 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.aplicar_desconto_venda).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.aplicar_desconto_venda));
            RuleFor(x => x.valor_desconto_subtotal).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_desconto_subtotal));
            RuleFor(x => x.valor_desconto_completo).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_desconto_completo));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}