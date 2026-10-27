using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ProductAttributes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.ProductAttributes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script010_Create_LinxMicrovix_Product_Attributes_Tables
    /// </summary>
    public class LinxLinhasValidator : AbstractValidator<LinxLinhas>
    {
        public LinxLinhasValidator()
        {
            RuleFor(x => x.id_linha)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_linha' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.desc_linha)
                .MaximumLength(30)
                .WithMessage("O campo 'desc_linha' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_linha));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.codigo_integracao_ws)
                .MaximumLength(50)
                .WithMessage("O campo 'codigo_integracao_ws' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_integracao_ws));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.coeficiente_comissao)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.coeficiente_comissao));
        }
    }
}
