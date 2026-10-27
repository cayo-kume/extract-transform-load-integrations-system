using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.ServiceOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script064_Create_LinxMicrovix_Service_Orders_Tables
    /// </summary>
    public class LinxPagamentoParcialDavValidator : AbstractValidator<LinxPagamentoParcialDav>
    {
        public LinxPagamentoParcialDavValidator()
        {
            RuleFor(x => x.id_vendas_pos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_vendas_pos));

            RuleFor(x => x.valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.ajuste_valor_desc_acresc_plano)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.ajuste_valor_desc_acresc_plano));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.id_pagamento_parcial_tmp)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_pagamento_parcial_tmp' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.plano)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano));

            RuleFor(x => x.forma_pgto)
                .MaximumLength(50)
                .WithMessage("O campo 'forma_pgto' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.forma_pgto));

            RuleFor(x => x.valor_2)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_2));

            RuleFor(x => x.id_bandeira)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_bandeira));

            RuleFor(x => x.qtde_parcelas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtde_parcelas));

            RuleFor(x => x.credito_debito)
                .MaximumLength(1)
                .WithMessage("O campo 'credito_debito' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.credito_debito));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
