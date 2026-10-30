using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Optics
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script067_Create_LinxMicrovix_Optics_Tables
    /// </summary>
    public class LinxProdutosOpticosTipoValidator : AbstractValidator<LinxProdutosOpticosTipo>
    {
        public LinxProdutosOpticosTipoValidator()
        {
            RuleFor(x => x.id_produtos_opticos_tipo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_produtos_opticos_tipo' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.tipo)
                .MaximumLength(100)
                .WithMessage("O campo 'tipo' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
