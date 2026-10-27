using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script076_Create_LinxMicrovix_B2C_Products_Tables
    /// </summary>
    public class B2CConsultaProdutosStatusValidator : AbstractValidator<B2CConsultaProdutosStatus>
    {
        public B2CConsultaProdutosStatusValidator()
        {
            RuleFor(x => x.codigoproduto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigoproduto' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.referencia)
                .MaximumLength(20)
                .WithMessage("O campo 'referencia' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.referencia));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.b2c)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.b2c));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
