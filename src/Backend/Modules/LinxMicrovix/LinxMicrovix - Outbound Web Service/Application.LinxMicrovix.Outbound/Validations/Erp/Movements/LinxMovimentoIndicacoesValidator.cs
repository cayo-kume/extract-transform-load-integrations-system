using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Movements
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script040_Create_LinxMicrovix_Movements_Tables
    /// </summary>
    public class LinxMovimentoIndicacoesValidator : AbstractValidator<LinxMovimentoIndicacoes>
    {
        public LinxMovimentoIndicacoesValidator()
        {
            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.cod_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
