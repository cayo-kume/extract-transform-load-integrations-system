using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script010_Create_LinxMicrovix_Product_Attributes_Tables
    /// </summary>
    public class LinxUnidadesValidator : AbstractValidator<LinxUnidades>
    {
        public LinxUnidadesValidator()
        {
            RuleFor(x => x.unidade)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'unidade' é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O campo 'unidade' deve ter no máximo 50 caracteres.");

            RuleFor(x => x.descricao)
                .MaximumLength(200)
                .WithMessage("O campo 'descricao' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
