using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductImages;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.ProductImages
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script079_Create_LinxMicrovix_B2C_Product_Images_Tables
    /// </summary>
    public class B2CConsultaProdutosImagensValidator : AbstractValidator<B2CConsultaProdutosImagens>
    {
        public B2CConsultaProdutosImagensValidator()
        {
            RuleFor(x => x.id_imagem_produto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_imagem_produto' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_imagem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_imagem));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.principal)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.principal));
        }
    }
}
