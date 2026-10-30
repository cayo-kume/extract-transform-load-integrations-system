using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Optics
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script067_Create_LinxMicrovix_Optics_Tables
    /// </summary>
    public class LinxProdutosOpticosFormatoAroValidator : AbstractValidator<LinxProdutosOpticosFormatoAro>
    {
        public LinxProdutosOpticosFormatoAroValidator()
        {
            RuleFor(x => x.id_formato_aro)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_formato_aro' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.codigo)
                .MaximumLength(16)
                .WithMessage("O campo 'codigo' deve ter no máximo 16 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo));

            RuleFor(x => x.descricao)
                .MaximumLength(60)
                .WithMessage("O campo 'descricao' deve ter no máximo 60 caracteres.")
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
