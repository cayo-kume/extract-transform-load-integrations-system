using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxEnquadramentoIpiValidator : AbstractValidator<LinxEnquadramentoIpi>
    {
        public LinxEnquadramentoIpiValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_enquadramento_ipi)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_enquadramento_ipi' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.codigo)
                .MaximumLength(3)
                .WithMessage("O campo 'codigo' deve ter no máximo 3 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo));

            RuleFor(x => x.descricao)
                .MaximumLength(600)
                .WithMessage("O campo 'descricao' deve ter no máximo 600 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
