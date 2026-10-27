using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxExtratoBancarioConciliacoesValidator : AbstractValidator<LinxExtratoBancarioConciliacoes>
    {
        public LinxExtratoBancarioConciliacoesValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_extrato_bancario_conciliacoes)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_extrato_bancario_conciliacoes' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_extrato_bancario_lancamentos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_extrato_bancario_lancamentos));

            RuleFor(x => x.codigo_fatura)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigo_fatura));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
