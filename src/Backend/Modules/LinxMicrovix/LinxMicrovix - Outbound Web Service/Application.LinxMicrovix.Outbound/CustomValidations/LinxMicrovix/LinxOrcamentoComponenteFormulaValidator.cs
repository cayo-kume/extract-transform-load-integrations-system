using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxOrcamentoComponenteFormulaValidator : AbstractValidator<LinxOrcamentoComponenteFormula>
    {
        public LinxOrcamentoComponenteFormulaValidator()
        {
            RuleFor(x => x.id_orcamento_componente_formula).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_orcamento_componente_formula));
            RuleFor(x => x.documento).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.documento));
            RuleFor(x => x.codigo_produto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigo_produto));
            RuleFor(x => x.codigo_componente).MaximumLength(10).WithMessage("O campo 'codigo_componente' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_componente));
            RuleFor(x => x.descricao_componente).MaximumLength(50).WithMessage("O campo 'descricao_componente' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_componente));
            RuleFor(x => x.unidade).MaximumLength(5).WithMessage("O campo 'unidade' deve ter no máximo 5 caracteres.").When(x => !string.IsNullOrEmpty(x.unidade));
            RuleFor(x => x.quantidade).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.quantidade));
            RuleFor(x => x.valor_componente).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_componente));
            RuleFor(x => x.lote_componente).MaximumLength(30).WithMessage("O campo 'lote_componente' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.lote_componente));
            RuleFor(x => x.data_validade_lote).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_validade_lote));
            RuleFor(x => x.codigo_ws).MaximumLength(50).WithMessage("O campo 'codigo_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_ws));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
