using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosDetalhesValidator : AbstractValidator<LinxProdutosDetalhes>
    {
        public LinxProdutosDetalhesValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.cod_produto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_produto));
            RuleFor(x => x.cod_barra).MaximumLength(20).WithMessage("O campo 'cod_barra' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.cod_barra));
            RuleFor(x => x.quantidade).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.quantidade));
            RuleFor(x => x.preco_custo).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.preco_custo));
            RuleFor(x => x.preco_venda).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.preco_venda));
            RuleFor(x => x.custo_medio).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.custo_medio));
            RuleFor(x => x.id_config_tributaria).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_config_tributaria));
            RuleFor(x => x.desc_config_tributaria).MaximumLength(100).WithMessage("O campo 'desc_config_tributaria' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_config_tributaria));
            RuleFor(x => x.despesas1).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.despesas1));
            RuleFor(x => x.qtde_minima).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_minima));
            RuleFor(x => x.qtde_maxima).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_maxima));
            RuleFor(x => x.ipi).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.ipi));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
        }
    }
}
