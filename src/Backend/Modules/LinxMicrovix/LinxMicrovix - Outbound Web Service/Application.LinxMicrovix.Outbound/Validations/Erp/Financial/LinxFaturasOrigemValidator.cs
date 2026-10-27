using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxFaturasOrigemValidator : AbstractValidator<LinxFaturasOrigem>
    {
        public LinxFaturasOrigemValidator()
        {
            RuleFor(x => x.codigo_fatura)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigo_fatura));

            RuleFor(x => x.codigo_fatura_origem)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigo_fatura_origem));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
