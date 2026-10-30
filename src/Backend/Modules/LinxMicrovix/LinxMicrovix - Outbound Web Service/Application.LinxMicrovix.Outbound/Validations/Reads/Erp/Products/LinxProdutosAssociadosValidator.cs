using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script019_Create_LinxMicrovix_Products_Tables
    /// </summary>
    public class LinxProdutosAssociadosValidator : AbstractValidator<LinxProdutosAssociados>
    {
        public LinxProdutosAssociadosValidator()
        {
            RuleFor(x => x.portal)
                .MaximumLength(50)
                .WithMessage("O campo 'portal' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.codigoproduto_origem)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto_origem));

            RuleFor(x => x.codigoproduto_associado)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto_associado));

            RuleFor(x => x.coeficiente_desconto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.coeficiente_desconto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.qtde_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_item));

            RuleFor(x => x.item_obrigatorio)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.item_obrigatorio));
        }
    }
}
