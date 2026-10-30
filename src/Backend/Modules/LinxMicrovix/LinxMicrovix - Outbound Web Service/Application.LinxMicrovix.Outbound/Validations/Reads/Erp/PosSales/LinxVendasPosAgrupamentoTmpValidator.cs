using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.PosSales
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script043_Create_LinxMicrovix_Pos_Sales_Tables
    /// </summary>
    public class LinxVendasPosAgrupamentoTmpValidator : AbstractValidator<LinxVendasPosAgrupamentoTmp>
    {
        public LinxVendasPosAgrupamentoTmpValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_vendas_pos_agrupamento_tmp)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_vendas_pos_agrupamento_tmp' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_vendas_pos)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id_vendas_pos));

            RuleFor(x => x.id_vendas_pos_origem)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id_vendas_pos_origem));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
