using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script019_Create_LinxMicrovix_Products_Tables
    /// </summary>
    public class LinxProdutosSerialValidator : AbstractValidator<LinxProdutosSerial>
    {
        public LinxProdutosSerialValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.serial)
                .MaximumLength(50)
                .WithMessage("O campo 'serial' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serial));

            RuleFor(x => x.id_deposito)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_deposito));

            RuleFor(x => x.saldo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.saldo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
