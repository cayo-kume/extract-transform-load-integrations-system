using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Stocks
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script022_Create_LinxMicrovix_Stocks_Tables
    /// </summary>
    public class LinxProdutosDepositosValidator : AbstractValidator<LinxProdutosDepositos>
    {
        public LinxProdutosDepositosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cod_deposito)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'cod_deposito' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_deposito)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_deposito' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_deposito));

            RuleFor(x => x.disponivel)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.disponivel));

            RuleFor(x => x.disponivel_transferencia)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.disponivel_transferencia));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.outlet)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.outlet));
        }
    }
}
