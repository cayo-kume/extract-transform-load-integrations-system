using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.ProductImages
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script079_Create_LinxMicrovix_B2C_Product_Images_Tables
    /// </summary>
    public class B2CConsultaImagensValidator : AbstractValidator<B2CConsultaImagens>
    {
        public B2CConsultaImagensValidator()
        {
            RuleFor(x => x.id_imagem)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_imagem' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.imagem)
                .MaximumLength(1000)
                .WithMessage("O campo 'imagem' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.imagem));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.url_imagem_blob)
                .MaximumLength(1000)
                .WithMessage("O campo 'url_imagem_blob' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.url_imagem_blob));
        }
    }
}
