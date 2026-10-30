using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.PosSales
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script043_Create_LinxMicrovix_Pos_Sales_Tables
    /// </summary>
    public class LinxVendasPosTmpValidator : AbstractValidator<LinxVendasPosTmp>
    {
        public LinxVendasPosTmpValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_vendas_pos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_vendas_pos' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.numero_ficha)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.numero_ficha));

            RuleFor(x => x.numero_sequencial)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_sequencial));

            RuleFor(x => x.cod_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente));

            RuleFor(x => x.data_lcto)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_lcto));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.cliente_indicador)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cliente_indicador));

            RuleFor(x => x.observacao_dav)
                .MaximumLength(4000)
                .WithMessage("O campo 'observacao_dav' deve ter no máximo 4000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observacao_dav));

            RuleFor(x => x.observacao)
                .MaximumLength(500)
                .WithMessage("O campo 'observacao' deve ter no máximo 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observacao));

            RuleFor(x => x.codigo_sistema_terceiro)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_sistema_terceiro));

            RuleFor(x => x.cancelado)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.cancelado));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
