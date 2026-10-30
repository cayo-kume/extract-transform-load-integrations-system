using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script019_Create_LinxMicrovix_Products_Tables
    /// </summary>
    public class LinxProdutosFornecValidator : AbstractValidator<LinxProdutosFornec>
    {
        public LinxProdutosFornecValidator()
        {
            RuleFor(x => x.id_prod_forn)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_prod_forn' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.cod_fornecedor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_fornecedor));

            RuleFor(x => x.custo)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.custo));

            RuleFor(x => x.moeda)
                .MaximumLength(10)
                .WithMessage("O campo 'moeda' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.moeda));

            RuleFor(x => x.unidade_compra)
                .MaximumLength(50)
                .WithMessage("O campo 'unidade_compra' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.unidade_compra));

            RuleFor(x => x.fator_conversao_compra)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.fator_conversao_compra));

            RuleFor(x => x.codauxiliar)
                .MaximumLength(40)
                .WithMessage("O campo 'codauxiliar' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codauxiliar));

            RuleFor(x => x.qtde_embalagem)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_embalagem));

            RuleFor(x => x.prazo_entrega_padrao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.prazo_entrega_padrao));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.desconto1)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto1));

            RuleFor(x => x.desconto2)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto2));

            RuleFor(x => x.desconto3)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto3));

            RuleFor(x => x.desconto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto));

            RuleFor(x => x.ipi1)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.ipi1));

            RuleFor(x => x.diferencial_icms)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.diferencial_icms));

            RuleFor(x => x.despesas1)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.despesas1));

            RuleFor(x => x.acrescimo)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.acrescimo));

            RuleFor(x => x.valor_custo_substituicao)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_custo_substituicao));

            RuleFor(x => x.frete1)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.frete1));

            RuleFor(x => x.fornecedor_principal)
                .MaximumLength(1)
                .WithMessage("O campo 'fornecedor_principal' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fornecedor_principal));

            RuleFor(x => x.excluido)
                .MaximumLength(1)
                .WithMessage("O campo 'excluido' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.excluido));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
