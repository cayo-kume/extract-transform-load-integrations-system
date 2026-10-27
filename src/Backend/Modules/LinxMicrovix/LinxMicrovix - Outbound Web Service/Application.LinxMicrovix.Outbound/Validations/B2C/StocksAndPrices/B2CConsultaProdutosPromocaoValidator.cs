using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.StocksAndPrices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script085_Create_LinxMicrovix_B2C_Stocks_And_Prices_Tables
    /// </summary>
    public class B2CConsultaProdutosPromocaoValidator : AbstractValidator<B2CConsultaProdutosPromocao>
    {
        public B2CConsultaProdutosPromocaoValidator()
        {
            RuleFor(x => x.codigo_promocao)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigo_promocao));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.preco)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco));

            RuleFor(x => x.data_inicio)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_inicio));

            RuleFor(x => x.data_termino)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_termino));

            RuleFor(x => x.data_cadastro)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_cadastro));

            RuleFor(x => x.ativa)
                .MaximumLength(1)
                .WithMessage("O campo 'ativa' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ativa));

            RuleFor(x => x.codigo_campanha)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_campanha));

            RuleFor(x => x.promocao_opcional)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.promocao_opcional));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.referencia)
                .MaximumLength(20)
                .WithMessage("O campo 'referencia' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.referencia));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
