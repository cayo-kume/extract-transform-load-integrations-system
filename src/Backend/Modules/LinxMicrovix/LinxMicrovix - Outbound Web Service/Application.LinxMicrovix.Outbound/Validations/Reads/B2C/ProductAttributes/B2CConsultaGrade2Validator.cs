using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script073_Create_LinxMicrovix_B2C_Product_Attributes_Tables
    /// </summary>
    public class B2CConsultaGrade2Validator : AbstractValidator<B2CConsultaGrade2>
    {
        public B2CConsultaGrade2Validator()
        {
            RuleFor(x => x.codigo_grade2)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigo_grade2' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_grade2)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_grade2' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_grade2));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
