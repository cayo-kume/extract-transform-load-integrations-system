using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxDesfazimentosAntecipacoesValidator : AbstractValidator<LinxDesfazimentosAntecipacoes>
    {
        public LinxDesfazimentosAntecipacoesValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_desfazimentos_antecipacoes)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_desfazimentos_antecipacoes' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_antecipacoes_financeiras)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_antecipacoes_financeiras));

            RuleFor(x => x.data_desfazimento)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_desfazimento));

            RuleFor(x => x.valor_desfazimento)
                .MaximumLength(1000)
                .WithMessage("O campo 'valor_desfazimento' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.valor_desfazimento));

            RuleFor(x => x.valor_acerto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_acerto));

            RuleFor(x => x.tipo_acerto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tipo_acerto));

            RuleFor(x => x.usuario)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.usuario));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
