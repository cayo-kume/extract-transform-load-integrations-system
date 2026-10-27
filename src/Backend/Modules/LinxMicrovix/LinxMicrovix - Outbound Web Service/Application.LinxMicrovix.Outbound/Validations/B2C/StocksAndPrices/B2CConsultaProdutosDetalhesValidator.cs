using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.StocksAndPrices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script085_Create_LinxMicrovix_B2C_Stocks_And_Prices_Tables
    /// </summary>
    public class B2CConsultaProdutosDetalhesValidator : AbstractValidator<B2CConsultaProdutosDetalhes>
    {
        public B2CConsultaProdutosDetalhesValidator()
        {
            RuleFor(x => x.id_prod_det)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_prod_det' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.saldo)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.saldo));

            RuleFor(x => x.controla_lote)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.controla_lote));

            RuleFor(x => x.nomeproduto_alternativo)
                .MaximumLength(250)
                .WithMessage("O campo 'nomeproduto_alternativo' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nomeproduto_alternativo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.referencia)
                .MaximumLength(50)
                .WithMessage("O campo 'referencia' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.referencia));

            RuleFor(x => x.localizacao)
                .MaximumLength(50)
                .WithMessage("O campo 'localizacao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.localizacao));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.tempo_preparacao_estoque)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tempo_preparacao_estoque));
        }
    }
}
