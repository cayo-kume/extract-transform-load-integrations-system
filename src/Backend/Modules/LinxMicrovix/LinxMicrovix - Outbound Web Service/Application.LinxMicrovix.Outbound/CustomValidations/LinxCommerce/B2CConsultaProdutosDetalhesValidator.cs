using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosDetalhesValidator : AbstractValidator<B2CConsultaProdutosDetalhes>
    {
        public B2CConsultaProdutosDetalhesValidator()
        {
            RuleFor(x => x.id_prod_det).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_prod_det));
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.saldo).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.saldo));
            RuleFor(x => x.controla_lote).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.controla_lote));
            RuleFor(x => x.nomeproduto_alternativo).MaximumLength(250).WithMessage("O campo 'nomeproduto_alternativo' deve ter no máximo 250 caracteres.").When(x => !string.IsNullOrEmpty(x.nomeproduto_alternativo));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.referencia).MaximumLength(20).WithMessage("O campo 'referencia' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.referencia));
            RuleFor(x => x.localizacao).MaximumLength(50).WithMessage("O campo 'localizacao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.localizacao));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.tempo_preparacao_estoque).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.tempo_preparacao_estoque));
        }
    }
}
