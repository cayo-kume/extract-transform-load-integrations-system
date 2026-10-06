using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxListaDaVezValidator : AbstractValidator<LinxListaDaVez>
    {
        public LinxListaDaVezValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.cod_vendedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_vendedor));
            RuleFor(x => x.data).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data));
            RuleFor(x => x.motivo_nao_venda).MaximumLength(103).WithMessage("O campo 'motivo_nao_venda' deve ter no máximo 103 caracteres.").When(x => !string.IsNullOrEmpty(x.motivo_nao_venda));
            RuleFor(x => x.qtde_ocorrencias).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtde_ocorrencias));
            RuleFor(x => x.data_hora_ini_atend).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_hora_ini_atend));
            RuleFor(x => x.data_hora_fim_atend).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_hora_fim_atend));
            RuleFor(x => x.desc_produto_neg).MaximumLength(50).WithMessage("O campo 'desc_produto_neg' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_produto_neg));
            RuleFor(x => x.valor_produto_neg).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_produto_neg));
        }
    }
}