using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxPlanosPedidoVendaValidator : AbstractValidator<LinxPlanosPedidoVenda>
    {
        public LinxPlanosPedidoVendaValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.cod_pedido).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_pedido));
            RuleFor(x => x.plano).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.plano));
            RuleFor(x => x.desc_plano).MaximumLength(35).WithMessage("O campo 'desc_plano' deve ter no máximo 35 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_plano));
            RuleFor(x => x.total).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total));
            RuleFor(x => x.qtde_parcelas).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtde_parcelas));
            RuleFor(x => x.indice_plano).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.indice_plano));
            RuleFor(x => x.valor_desc_acresc_plano).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_desc_acresc_plano));
            RuleFor(x => x.cod_forma_pgto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_forma_pgto));
            RuleFor(x => x.forma_pgto).MaximumLength(50).WithMessage("O campo 'forma_pgto' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.forma_pgto));
        }
    }
}
