using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Prices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script025_Create_LinxMicrovix_Prices_Tables
    /// </summary>
    public class LinxProdutosTabelasPrecosValidator : AbstractValidator<LinxProdutosTabelasPrecos>
    {
        public LinxProdutosTabelasPrecosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.id_tabela)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_tabela));

            RuleFor(x => x.cod_produto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.cod_produto));

            RuleFor(x => x.precovenda)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.precovenda));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
