using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxSpedTipoItemValidator : AbstractValidator<LinxSpedTipoItem>
    {
        public LinxSpedTipoItemValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_sped_tipo_item)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_sped_tipo_item' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.codigo)
                .MaximumLength(2)
                .WithMessage("O campo 'codigo' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo));

            RuleFor(x => x.descricao)
                .MaximumLength(60)
                .WithMessage("O campo 'descricao' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
