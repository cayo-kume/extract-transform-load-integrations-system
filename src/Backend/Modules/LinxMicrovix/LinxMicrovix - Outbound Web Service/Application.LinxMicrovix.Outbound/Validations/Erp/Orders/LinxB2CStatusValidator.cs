using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Orders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script052_Create_LinxMicrovix_Orders_Tables
    /// </summary>
    public class LinxB2CStatusValidator : AbstractValidator<LinxB2CStatus>
    {
        public LinxB2CStatusValidator()
        {
            RuleFor(x => x.id_status)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_status' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao_status)
                .MaximumLength(30)
                .WithMessage("O campo 'descricao_status' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_status));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
