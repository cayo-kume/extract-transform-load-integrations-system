using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosValidator : AbstractValidator<B2CConsultaProdutos>
    {
        public B2CConsultaProdutosValidator()
        {
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.referencia).MaximumLength(20).WithMessage("O campo 'referencia' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.referencia));
            RuleFor(x => x.codauxiliar1).MaximumLength(40).WithMessage("O campo 'codauxiliar1' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.codauxiliar1));
            RuleFor(x => x.descricao_basica).MaximumLength(100).WithMessage("O campo 'descricao_basica' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_basica));
            RuleFor(x => x.nome_produto).MaximumLength(250).WithMessage("O campo 'nome_produto' deve ter no máximo 250 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_produto));
            RuleFor(x => x.peso_liquido).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.peso_liquido));
            RuleFor(x => x.codigo_setor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_setor));
            RuleFor(x => x.codigo_linha).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_linha));
            RuleFor(x => x.codigo_marca).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_marca));
            RuleFor(x => x.codigo_colecao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_colecao));
            RuleFor(x => x.codigo_espessura).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_espessura));
            RuleFor(x => x.codigo_grade1).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_grade1));
            RuleFor(x => x.codigo_grade2).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_grade2));
            RuleFor(x => x.unidade).MaximumLength(50).WithMessage("O campo 'unidade' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.unidade));
            RuleFor(x => x.ativo).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.codigo_classificacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_classificacao));
            RuleFor(x => x.dt_cadastro).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.dt_cadastro));
            RuleFor(x => x.cod_fornecedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_fornecedor));
            RuleFor(x => x.dt_update).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.dt_update));
            RuleFor(x => x.altura_para_frete).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.altura_para_frete));
            RuleFor(x => x.largura_para_frete).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.largura_para_frete));
            RuleFor(x => x.comprimento_para_frete).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.comprimento_para_frete));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.peso_bruto).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.peso_bruto));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.descricao_completa_commerce).MaximumLength(8000).WithMessage("O campo 'descricao_completa_commerce' deve ter no máximo 8000 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_completa_commerce));
            RuleFor(x => x.canais_ecommerce_publicados).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.canais_ecommerce_publicados));
            RuleFor(x => x.inicio_publicacao_produto).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.inicio_publicacao_produto));
            RuleFor(x => x.fim_publicacao_produto).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.fim_publicacao_produto));
            RuleFor(x => x.codigo_integracao_oms).MaximumLength(50).WithMessage("O campo 'codigo_integracao_oms' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_integracao_oms));
        }
    }
}
