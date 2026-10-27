using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Stores
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script004_Create_LinxMicrovix_Stores_Tables
    /// </summary>
    public class LinxLojasParametrosValidator : AbstractValidator<LinxLojasParametros>
    {
        public LinxLojasParametrosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.percentual_minimo_antecipacao)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentual_minimo_antecipacao));

            RuleFor(x => x.timestamp)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'timestamp' é obrigatório.")
                .MustBeValidInt64();
        }
    }
}
