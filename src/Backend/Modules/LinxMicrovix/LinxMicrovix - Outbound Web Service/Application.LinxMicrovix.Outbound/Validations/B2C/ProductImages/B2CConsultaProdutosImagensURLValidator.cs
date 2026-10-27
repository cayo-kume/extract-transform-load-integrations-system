using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.ProductImages
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script079_Create_LinxMicrovix_B2C_Product_Images_Tables
    /// </summary>
    public class B2CConsultaProdutosImagensURLValidator : AbstractValidator<B2CConsultaProdutosImagensURL>
    {
        public B2CConsultaProdutosImagensURLValidator()
        {
            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.url_imagem_blob)
                .MaximumLength(200)
                .WithMessage("O campo 'url_imagem_blob' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.url_imagem_blob));

            RuleFor(x => x.tipo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tipo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
