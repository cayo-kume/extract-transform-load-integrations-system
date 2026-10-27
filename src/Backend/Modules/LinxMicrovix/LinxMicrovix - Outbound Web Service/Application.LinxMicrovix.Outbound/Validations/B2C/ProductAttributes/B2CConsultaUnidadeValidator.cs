using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script073_Create_LinxMicrovix_B2C_Product_Attributes_Tables
    /// </summary>
    public class B2CConsultaUnidadeValidator : AbstractValidator<B2CConsultaUnidade>
    {
        public B2CConsultaUnidadeValidator()
        {
            RuleFor(x => x.unidade)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'unidade' é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O campo 'unidade' deve ter no máximo 50 caracteres.");

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
