using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Products;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.Products
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script076_Create_LinxMicrovix_B2C_Products_Tables
    /// </summary>
    public class B2CConsultaProdutosFlagsValidator : AbstractValidator<B2CConsultaProdutosFlags>
    {
        public B2CConsultaProdutosFlagsValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_b2c_flags_produtos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_b2c_flags_produtos' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_b2c_flags)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_b2c_flags));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.descricao_b2c_flags)
                .MaximumLength(300)
                .WithMessage("O campo 'descricao_b2c_flags' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_b2c_flags));
        }
    }
}
