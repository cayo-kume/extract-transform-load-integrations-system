using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stocks;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Stocks
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script022_Create_LinxMicrovix_Stocks_Tables
    /// </summary>
    public class LinxProdutosDetalhesDepositosValidator : AbstractValidator<LinxProdutosDetalhesDepositos>
    {
        public LinxProdutosDetalhesDepositosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.cod_produto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.cod_produto));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.cod_deposito)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_deposito));

            RuleFor(x => x.saldo)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.saldo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
