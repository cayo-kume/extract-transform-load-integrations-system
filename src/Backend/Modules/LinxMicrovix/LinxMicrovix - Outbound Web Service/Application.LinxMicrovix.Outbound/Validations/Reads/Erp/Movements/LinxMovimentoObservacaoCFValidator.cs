using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Movements
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script040_Create_LinxMicrovix_Movements_Tables
    /// </summary>
    public class LinxMovimentoObservacaoCFValidator : AbstractValidator<LinxMovimentoObservacaoCF>
    {
        public LinxMovimentoObservacaoCFValidator()
        {
            RuleFor(x => x.identificador)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'identificador' é obrigatório.")
                .MustBeValidGuid();

            RuleFor(x => x.documento_cliente)
                .MaximumLength(14)
                .WithMessage("O campo 'documento_cliente' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.documento_cliente));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
