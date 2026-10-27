using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Sellers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script031_Create_LinxMicrovix_Sellers_Tables
    /// </summary>
    public class LinxComissoesVendedoresValidator : AbstractValidator<LinxComissoesVendedores>
    {
        public LinxComissoesVendedoresValidator()
        {
            RuleFor(x => x.vendedor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.vendedor));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.data_origem)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_origem));

            RuleFor(x => x.valor_base)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_base));

            RuleFor(x => x.valor_comissao)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_comissao));

            RuleFor(x => x.cancelado)
                .MaximumLength(1)
                .WithMessage("O campo 'cancelado' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cancelado));

            RuleFor(x => x.disponivel)
                .MaximumLength(1)
                .WithMessage("O campo 'disponivel' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.disponivel));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
