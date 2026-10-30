using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script073_Create_LinxMicrovix_B2C_Product_Attributes_Tables
    /// </summary>
    public class B2CConsultaGrade1Validator : AbstractValidator<B2CConsultaGrade1>
    {
        public B2CConsultaGrade1Validator()
        {
            RuleFor(x => x.codigo_grade1)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigo_grade1' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_grade1)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_grade1' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_grade1));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
