using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaImagensValidator : AbstractValidator<B2CConsultaImagens>
    {
        public B2CConsultaImagensValidator()
        {
            RuleFor(x => x.id_imagem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_imagem));

            RuleFor(x => x.md5)
                .MaximumLength(32)
                .WithMessage("O campo 'md5' deve ter no máximo 32 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.md5));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.url_imagem_blob)
                .MaximumLength(4000)
                .WithMessage("O campo 'url_imagem_blob' deve ter no máximo 4000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.url_imagem_blob));
        }
    }
}
