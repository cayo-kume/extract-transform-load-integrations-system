using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Movements
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script040_Create_LinxMicrovix_Movements_Tables
    /// </summary>
    public class LinxRespostaVendaValidator : AbstractValidator<LinxRespostaVenda>
    {
        public LinxRespostaVendaValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_resposta_venda)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_resposta_venda' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao_resposta)
                .MaximumLength(100)
                .WithMessage("O campo 'descricao_resposta' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_resposta));

            RuleFor(x => x.id_pergunta_venda)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_pergunta_venda));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
