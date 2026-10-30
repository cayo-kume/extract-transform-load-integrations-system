using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script019_Create_LinxMicrovix_Products_Tables
    /// </summary>
    public class LinxProdutosCodBarValidator : AbstractValidator<LinxProdutosCodBar>
    {
        public LinxProdutosCodBarValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cod_produto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.cod_produto));

            RuleFor(x => x.cod_barra)
                .MaximumLength(20)
                .WithMessage("O campo 'cod_barra' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cod_barra));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
