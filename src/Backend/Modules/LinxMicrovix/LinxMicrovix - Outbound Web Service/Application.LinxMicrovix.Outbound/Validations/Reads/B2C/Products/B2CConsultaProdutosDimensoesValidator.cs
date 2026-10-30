using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script076_Create_LinxMicrovix_B2C_Products_Tables
    /// </summary>
    public class B2CConsultaProdutosDimensoesValidator : AbstractValidator<B2CConsultaProdutosDimensoes>
    {
        public B2CConsultaProdutosDimensoesValidator()
        {
            RuleFor(x => x.codigoproduto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigoproduto' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.altura)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.altura));

            RuleFor(x => x.comprimento)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.comprimento));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.largura)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.largura));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
