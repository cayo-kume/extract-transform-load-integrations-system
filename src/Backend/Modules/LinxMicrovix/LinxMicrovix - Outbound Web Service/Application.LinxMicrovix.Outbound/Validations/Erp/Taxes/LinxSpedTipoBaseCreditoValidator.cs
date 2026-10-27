using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxSpedTipoBaseCreditoValidator : AbstractValidator<LinxSpedTipoBaseCredito>
    {
        public LinxSpedTipoBaseCreditoValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_sped_tipo_base_credito)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_sped_tipo_base_credito' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.codigo_sped_tipo_base_credito)
                .MaximumLength(2)
                .WithMessage("O campo 'codigo_sped_tipo_base_credito' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_sped_tipo_base_credito));

            RuleFor(x => x.descricao)
                .MaximumLength(150)
                .WithMessage("O campo 'descricao' deve ter no máximo 150 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
