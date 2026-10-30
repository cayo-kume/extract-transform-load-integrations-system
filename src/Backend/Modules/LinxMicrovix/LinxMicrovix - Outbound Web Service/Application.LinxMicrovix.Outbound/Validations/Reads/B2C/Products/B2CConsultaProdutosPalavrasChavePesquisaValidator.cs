using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script076_Create_LinxMicrovix_B2C_Products_Tables
    /// </summary>
    public class B2CConsultaProdutosPalavrasChavePesquisaValidator : AbstractValidator<B2CConsultaProdutosPalavrasChavePesquisa>
    {
        public B2CConsultaProdutosPalavrasChavePesquisaValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_b2c_palavras_chave_pesquisa_produtos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_b2c_palavras_chave_pesquisa_produtos' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_b2c_palavras_chave_pesquisa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_b2c_palavras_chave_pesquisa));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
