using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Prices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script025_Create_LinxMicrovix_Prices_Tables
    /// </summary>
    public class LinxCupomDescontoVendasValidator : AbstractValidator<LinxCupomDescontoVendas>
    {
        public LinxCupomDescontoVendasValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_cupom_desconto_vendas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cupom_desconto_vendas));

            RuleFor(x => x.id_cupom_desconto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cupom_desconto));

            RuleFor(x => x.identificador)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'identificador' é obrigatório.")
                .MustBeValidGuid();

            RuleFor(x => x.valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.id_vendas_pos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_vendas_pos));
        }
    }
}
