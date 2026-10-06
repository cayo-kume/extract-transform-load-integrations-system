using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaImagensHDValidator : AbstractValidator<B2CConsultaImagensHD>
    {
        public B2CConsultaImagensHDValidator()
        {
            RuleFor(x => x.identificador_imagem)
                .Must(value => Guid.TryParse(value, out _))
                .WithMessage("O campo 'identificador_imagem' deve ser um GUID válido.")
                .When(x => !string.IsNullOrEmpty(x.identificador_imagem));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.url_imagem_blob)
                .MaximumLength(200)
                .WithMessage("O campo 'url_imagem_blob' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.url_imagem_blob));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
