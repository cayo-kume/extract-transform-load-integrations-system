using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script019_Create_LinxMicrovix_Products_Tables
    /// </summary>
    public class LinxProdutosInformacoesValidator : AbstractValidator<LinxProdutosInformacoes>
    {
        public LinxProdutosInformacoesValidator()
        {
            RuleFor(x => x.codigoproduto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigoproduto' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
