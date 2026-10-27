using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Orders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script052_Create_LinxMicrovix_Orders_Tables
    /// </summary>
    public class LinxOrcamentoPosicaoValidator : AbstractValidator<LinxOrcamentoPosicao>
    {
        public LinxOrcamentoPosicaoValidator()
        {
            RuleFor(x => x.id_posicao)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_posicao' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao_posicao)
                .MaximumLength(50)
                .WithMessage("O campo 'descricao_posicao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_posicao));

            RuleFor(x => x.ativo_posicao)
                .MaximumLength(1)
                .WithMessage("O campo 'ativo_posicao' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ativo_posicao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
