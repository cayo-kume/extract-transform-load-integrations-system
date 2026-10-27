using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.StocksAndPrices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script085_Create_LinxMicrovix_B2C_Stocks_And_Prices_Tables
    /// </summary>
    public class B2CConsultaProdutosDetalhesDepositosValidator : AbstractValidator<B2CConsultaProdutosDetalhesDepositos>
    {
        public B2CConsultaProdutosDetalhesDepositosValidator()
        {
            RuleFor(x => x.codigoproduto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigoproduto' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_deposito)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_deposito));

            RuleFor(x => x.saldo)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.saldo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.tempo_preparacao_estoque)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tempo_preparacao_estoque));
        }
    }
}
