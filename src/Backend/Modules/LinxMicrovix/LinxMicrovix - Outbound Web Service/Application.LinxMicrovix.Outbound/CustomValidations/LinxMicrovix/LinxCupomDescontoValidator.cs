using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxCupomDescontoValidator : AbstractValidator<LinxCupomDesconto>
    {
        public LinxCupomDescontoValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.id_cupom_desconto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_cupom_desconto));
            RuleFor(x => x.cupom).MaximumLength(50).WithMessage("O campo 'cupom' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.cupom));
            RuleFor(x => x.descricao).MaximumLength(255).WithMessage("O campo 'descricao' deve ter no máximo 255 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.percentual_desconto).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.percentual_desconto));
            RuleFor(x => x.data_inicial_vigencia).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_inicial_vigencia));
            RuleFor(x => x.data_final_vigencia).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_final_vigencia));
            RuleFor(x => x.qtde_original).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtde_original));
            RuleFor(x => x.qtde_disponivel).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtde_disponivel));
            RuleFor(x => x.cod_vendedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_vendedor));
            RuleFor(x => x.disponivel).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.disponivel));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
