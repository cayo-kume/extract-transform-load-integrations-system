using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.StocksAndPrices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script085_Create_LinxMicrovix_B2C_Stocks_And_Prices_Tables
    /// </summary>
    public class B2CConsultaProdutosTabelasPrecosValidator : AbstractValidator<B2CConsultaProdutosTabelasPrecos>
    {
        public B2CConsultaProdutosTabelasPrecosValidator()
        {
            RuleFor(x => x.id_prod_tab_preco)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_prod_tab_preco));

            RuleFor(x => x.id_tabela)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_tabela));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.precovenda)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.precovenda));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
