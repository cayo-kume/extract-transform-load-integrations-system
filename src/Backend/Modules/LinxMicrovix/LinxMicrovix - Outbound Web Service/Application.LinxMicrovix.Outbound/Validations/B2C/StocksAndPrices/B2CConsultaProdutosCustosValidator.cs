using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.StocksAndPrices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script085_Create_LinxMicrovix_B2C_Stocks_And_Prices_Tables
    /// </summary>
    public class B2CConsultaProdutosCustosValidator : AbstractValidator<B2CConsultaProdutosCustos>
    {
        public B2CConsultaProdutosCustosValidator()
        {
            RuleFor(x => x.id_produtos_custos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_produtos_custos));

            RuleFor(x => x.codigoproduto)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigoproduto' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.custoicms1)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.custoicms1));

            RuleFor(x => x.ipi1)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.ipi1));

            RuleFor(x => x.markup)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.markup));

            RuleFor(x => x.customedio)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.customedio));

            RuleFor(x => x.frete1)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.frete1));

            RuleFor(x => x.precisao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.precisao));

            RuleFor(x => x.precominimo)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.precominimo));

            RuleFor(x => x.dt_update)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_update));

            RuleFor(x => x.custoliquido)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.custoliquido));

            RuleFor(x => x.precovenda)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.precovenda));

            RuleFor(x => x.custototal)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.custototal));

            RuleFor(x => x.precocompra)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.precocompra));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.desconto_especial)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto_especial));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
