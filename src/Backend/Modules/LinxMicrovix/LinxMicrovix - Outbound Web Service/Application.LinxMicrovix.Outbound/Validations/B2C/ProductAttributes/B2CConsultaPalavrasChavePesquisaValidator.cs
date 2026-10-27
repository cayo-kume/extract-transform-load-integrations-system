using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script073_Create_LinxMicrovix_B2C_Product_Attributes_Tables
    /// </summary>
    public class B2CConsultaPalavrasChavePesquisaValidator : AbstractValidator<B2CConsultaPalavrasChavePesquisa>
    {
        public B2CConsultaPalavrasChavePesquisaValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_b2c_palavras_chave_pesquisa)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_b2c_palavras_chave_pesquisa' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
