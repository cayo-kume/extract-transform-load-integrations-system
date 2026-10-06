using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaColecoesValidator : AbstractValidator<B2CConsultaColecoes>
    {
        public B2CConsultaColecoesValidator()
        {
            RuleFor(x => x.codigo_colecao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_colecao));

            RuleFor(x => x.nome_colecao)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_colecao' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_colecao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.marcas)
                .MaximumLength(250)
                .WithMessage("O campo 'marcas' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.marcas));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
