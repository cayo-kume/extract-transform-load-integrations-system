using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script073_Create_LinxMicrovix_B2C_Product_Attributes_Tables
    /// </summary>
    public class B2CConsultaLinhasValidator : AbstractValidator<B2CConsultaLinhas>
    {
        public B2CConsultaLinhasValidator()
        {
            RuleFor(x => x.codigo_linha)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigo_linha' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_linha)
                .MaximumLength(30)
                .WithMessage("O campo 'nome_linha' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_linha));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.setores)
                .MaximumLength(250)
                .WithMessage("O campo 'setores' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.setores));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
