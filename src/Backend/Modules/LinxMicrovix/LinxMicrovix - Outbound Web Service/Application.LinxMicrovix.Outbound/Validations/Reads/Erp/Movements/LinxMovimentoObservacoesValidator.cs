using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Movements
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script040_Create_LinxMicrovix_Movements_Tables
    /// </summary>
    public class LinxMovimentoObservacoesValidator : AbstractValidator<LinxMovimentoObservacoes>
    {
        public LinxMovimentoObservacoesValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_obs)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_obs' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.titulo_obs)
                .MaximumLength(50)
                .WithMessage("O campo 'titulo_obs' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.titulo_obs));

            RuleFor(x => x.descricao_obs)
                .MaximumLength(250)
                .WithMessage("O campo 'descricao_obs' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_obs));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
