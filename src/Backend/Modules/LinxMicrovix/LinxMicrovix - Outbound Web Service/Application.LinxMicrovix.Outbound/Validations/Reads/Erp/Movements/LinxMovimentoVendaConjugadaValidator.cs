using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Movements
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script040_Create_LinxMicrovix_Movements_Tables
    /// </summary>
    public class LinxMovimentoVendaConjugadaValidator : AbstractValidator<LinxMovimentoVendaConjugada>
    {
        public LinxMovimentoVendaConjugadaValidator()
        {
            RuleFor(x => x.identificador_produto)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_produto));

            RuleFor(x => x.identificador_servico)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_servico));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
