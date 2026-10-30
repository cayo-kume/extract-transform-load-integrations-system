using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Payments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script034_Create_LinxMicrovix_Payments_Tables
    /// </summary>
    public class LinxPlanosBandeirasValidator : AbstractValidator<LinxPlanosBandeiras>
    {
        public LinxPlanosBandeirasValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.plano)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano));

            RuleFor(x => x.bandeira)
                .MaximumLength(30)
                .WithMessage("O campo 'bandeira' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bandeira));

            RuleFor(x => x.tipo_bandeira)
                .MaximumLength(100)
                .WithMessage("O campo 'tipo_bandeira' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_bandeira));

            RuleFor(x => x.adquirente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.adquirente));

            RuleFor(x => x.nome_adquirente)
                .MaximumLength(60)
                .WithMessage("O campo 'nome_adquirente' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_adquirente));

            RuleFor(x => x.codigo_bandeira_sitef)
                .MaximumLength(10)
                .WithMessage("O campo 'codigo_bandeira_sitef' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_bandeira_sitef));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
