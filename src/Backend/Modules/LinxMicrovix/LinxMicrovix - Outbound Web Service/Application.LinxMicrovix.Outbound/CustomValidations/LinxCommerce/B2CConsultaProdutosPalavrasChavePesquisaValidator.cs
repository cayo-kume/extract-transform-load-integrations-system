using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaProdutosPalavrasChavePesquisaValidator : AbstractValidator<B2CConsultaProdutosPalavrasChavePesquisa>
    {
        public B2CConsultaProdutosPalavrasChavePesquisaValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_b2c_palavras_chave_pesquisa_produtos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_b2c_palavras_chave_pesquisa_produtos));

            RuleFor(x => x.id_b2c_palavras_chave_pesquisa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_b2c_palavras_chave_pesquisa));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.descricao_b2c_palavras_chave_pesquisa)
                .MaximumLength(300)
                .WithMessage("O campo 'descricao_b2c_palavras_chave_pesquisa' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_b2c_palavras_chave_pesquisa));
        }
    }
}
