using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Optics
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script067_Create_LinxMicrovix_Optics_Tables
    /// </summary>
    public class LinxProdutosOpticosTipoAroValidator : AbstractValidator<LinxProdutosOpticosTipoAro>
    {
        public LinxProdutosOpticosTipoAroValidator()
        {
            RuleFor(x => x.id_tipo_aro)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_tipo_aro' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.tipo_aro)
                .MaximumLength(100)
                .WithMessage("O campo 'tipo_aro' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_aro));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
