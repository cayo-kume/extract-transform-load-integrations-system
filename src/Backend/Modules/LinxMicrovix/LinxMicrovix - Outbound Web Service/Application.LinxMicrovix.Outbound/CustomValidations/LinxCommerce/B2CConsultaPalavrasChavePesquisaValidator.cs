using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaPalavrasChavePesquisaValidator : AbstractValidator<B2CConsultaPalavrasChavePesquisa>
    {
        public B2CConsultaPalavrasChavePesquisaValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_b2c_palavras_chave_pesquisa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_b2c_palavras_chave_pesquisa));

            RuleFor(x => x.nome_colecao)
                .MaximumLength(300)
                .WithMessage("O campo 'nome_colecao' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_colecao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
