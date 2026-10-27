using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Payments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script034_Create_LinxMicrovix_Payments_Tables
    /// </summary>
    public class LinxPlanosParcelasValidator : AbstractValidator<LinxPlanosParcelas>
    {
        public LinxPlanosParcelasValidator()
        {
            RuleFor(x => x.plano)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano));

            RuleFor(x => x.ordem_parcela)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ordem_parcela));

            RuleFor(x => x.prazo_parc)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.prazo_parc));

            RuleFor(x => x.id_planos_parcelas)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_planos_parcelas' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
