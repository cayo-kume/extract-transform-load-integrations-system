using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script019_Create_LinxMicrovix_Products_Tables
    /// </summary>
    public class LinxProdutosLotesValidator : AbstractValidator<LinxProdutosLotes>
    {
        public LinxProdutosLotesValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.lote)
                .MaximumLength(60)
                .WithMessage("O campo 'lote' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.lote));

            RuleFor(x => x.deposito)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.deposito));

            RuleFor(x => x.saldo_disponivel)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.saldo_disponivel));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
