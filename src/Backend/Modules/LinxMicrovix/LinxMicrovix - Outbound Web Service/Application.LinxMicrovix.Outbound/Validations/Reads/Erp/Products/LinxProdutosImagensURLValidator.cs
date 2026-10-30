using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script019_Create_LinxMicrovix_Products_Tables
    /// </summary>
    public class LinxProdutosImagensURLValidator : AbstractValidator<LinxProdutosImagensURL>
    {
        public LinxProdutosImagensURLValidator()
        {
            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.url_imagem_blob)
                .MaximumLength(200)
                .WithMessage("O campo 'url_imagem_blob' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.url_imagem_blob));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_imagem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_imagem));
        }
    }
}
