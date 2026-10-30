using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script073_Create_LinxMicrovix_B2C_Product_Attributes_Tables
    /// </summary>
    public class B2CConsultaMarcasValidator : AbstractValidator<B2CConsultaMarcas>
    {
        public B2CConsultaMarcasValidator()
        {
            RuleFor(x => x.codigo_marca)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigo_marca' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_marca)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_marca' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_marca));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.linhas)
                .MaximumLength(10)
                .WithMessage("O campo 'linhas' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.linhas));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
