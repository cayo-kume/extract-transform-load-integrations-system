using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.ProductFields
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script082_Create_LinxMicrovix_B2C_Product_Fields_Tables
    /// </summary>
    public class B2CConsultaProdutosCamposAdicionaisValoresValidator : AbstractValidator<B2CConsultaProdutosCamposAdicionaisValores>
    {
        public B2CConsultaProdutosCamposAdicionaisValoresValidator()
        {
            RuleFor(x => x.id_campo_valor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_campo_valor));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.id_campo_detalhe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_campo_detalhe));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
