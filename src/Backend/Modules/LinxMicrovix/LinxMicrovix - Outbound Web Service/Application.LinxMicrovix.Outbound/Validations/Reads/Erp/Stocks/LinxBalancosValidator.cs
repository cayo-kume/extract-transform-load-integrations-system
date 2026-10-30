using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Stocks
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script022_Create_LinxMicrovix_Stocks_Tables
    /// </summary>
    public class LinxBalancosValidator : AbstractValidator<LinxBalancos>
    {
        public LinxBalancosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_balancos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_balancos' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(250)
                .WithMessage("O campo 'descricao' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.data_lancamento)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_lancamento));

            RuleFor(x => x.status)
                .MaximumLength(100)
                .WithMessage("O campo 'status' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.id_deposito)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_deposito));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
