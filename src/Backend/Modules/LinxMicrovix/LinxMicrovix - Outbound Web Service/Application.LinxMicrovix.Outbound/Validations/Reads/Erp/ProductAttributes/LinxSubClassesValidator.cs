using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script010_Create_LinxMicrovix_Product_Attributes_Tables
    /// </summary>
    public class LinxSubClassesValidator : AbstractValidator<LinxSubClasses>
    {
        public LinxSubClassesValidator()
        {
            RuleFor(x => x.id_subclasses)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_subclasses' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.classe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.classe));

            RuleFor(x => x.descricao)
                .MaximumLength(50)
                .WithMessage("O campo 'descricao' deve ter no máximo 50 caracteres.")
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
