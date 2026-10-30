using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Movements
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script040_Create_LinxMicrovix_Movements_Tables
    /// </summary>
    public class LinxPerguntaVendaValidator : AbstractValidator<LinxPerguntaVenda>
    {
        public LinxPerguntaVendaValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_pergunta_venda)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_pergunta_venda' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao_pergunta)
                .MaximumLength(200)
                .WithMessage("O campo 'descricao_pergunta' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_pergunta));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
