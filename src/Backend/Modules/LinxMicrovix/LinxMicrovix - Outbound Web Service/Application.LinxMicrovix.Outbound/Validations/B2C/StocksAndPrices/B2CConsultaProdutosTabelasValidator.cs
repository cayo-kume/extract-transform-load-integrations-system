using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.StocksAndPrices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.StocksAndPrices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script085_Create_LinxMicrovix_B2C_Stocks_And_Prices_Tables
    /// </summary>
    public class B2CConsultaProdutosTabelasValidator : AbstractValidator<B2CConsultaProdutosTabelas>
    {
        public B2CConsultaProdutosTabelasValidator()
        {
            RuleFor(x => x.id_tabela)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_tabela' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_tabela)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_tabela' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_tabela));

            RuleFor(x => x.ativa)
                .MaximumLength(1)
                .WithMessage("O campo 'ativa' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ativa));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
