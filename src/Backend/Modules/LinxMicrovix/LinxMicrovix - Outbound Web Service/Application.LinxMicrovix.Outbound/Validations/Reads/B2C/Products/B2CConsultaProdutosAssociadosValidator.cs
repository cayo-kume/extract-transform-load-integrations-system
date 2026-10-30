using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script076_Create_LinxMicrovix_B2C_Products_Tables
    /// </summary>
    public class B2CConsultaProdutosAssociadosValidator : AbstractValidator<B2CConsultaProdutosAssociados>
    {
        public B2CConsultaProdutosAssociadosValidator()
        {
            RuleFor(x => x.id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.codigoproduto_associado)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto_associado));

            RuleFor(x => x.coeficiente_desconto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.coeficiente_desconto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.qtde_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_item));

            RuleFor(x => x.item_obrigatorio)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.item_obrigatorio));
        }
    }
}
