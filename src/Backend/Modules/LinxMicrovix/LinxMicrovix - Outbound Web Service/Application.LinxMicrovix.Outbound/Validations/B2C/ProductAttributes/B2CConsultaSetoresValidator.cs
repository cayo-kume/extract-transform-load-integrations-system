using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script073_Create_LinxMicrovix_B2C_Product_Attributes_Tables
    /// </summary>
    public class B2CConsultaSetoresValidator : AbstractValidator<B2CConsultaSetores>
    {
        public B2CConsultaSetoresValidator()
        {
            RuleFor(x => x.codigo_setor)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigo_setor' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_setor)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_setor' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_setor));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
