using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosPromocoesValidator : AbstractValidator<LinxProdutosPromocoes>
    {
        public LinxProdutosPromocoesValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.cod_produto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_produto));
            RuleFor(x => x.preco_promocao).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.preco_promocao));
            RuleFor(x => x.data_inicio_promocao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_inicio_promocao));
            RuleFor(x => x.data_termino_promocao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_termino_promocao));
            RuleFor(x => x.data_cadastro_promocao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_cadastro_promocao));
            RuleFor(x => x.promocao_ativa).MaximumLength(1).WithMessage("O campo 'promocao_ativa' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.promocao_ativa));
            RuleFor(x => x.id_campanha).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.id_campanha));
            RuleFor(x => x.nome_campanha).MaximumLength(60).WithMessage("O campo 'nome_campanha' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_campanha));
            RuleFor(x => x.promocao_opcional).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.promocao_opcional));
            RuleFor(x => x.custo_total_campanha).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.custo_total_campanha));
        }
    }
}
