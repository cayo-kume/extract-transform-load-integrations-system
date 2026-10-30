using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script064_Create_LinxMicrovix_Service_Orders_Tables
    /// </summary>
    public class LinxServicoClientesValidator : AbstractValidator<LinxServicoClientes>
    {
        public LinxServicoClientesValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cliente));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.id_servico)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id_servico));

            RuleFor(x => x.qtd_adquirido)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtd_adquirido));

            RuleFor(x => x.qtd_agendado)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtd_agendado));

            RuleFor(x => x.qtd_utilizado)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtd_utilizado));

            RuleFor(x => x.qtd_cancelado)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtd_cancelado));

            RuleFor(x => x.dt_insercao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_insercao));

            RuleFor(x => x.tipo_desconto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tipo_desconto));

            RuleFor(x => x.valor_desconto_por_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_desconto_por_item));

            RuleFor(x => x.id_dav_item)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id_dav_item));

            RuleFor(x => x.transacao_movimento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao_movimento));

            RuleFor(x => x.ficha)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ficha));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.identificador)
                .MaximumLength(1000)
                .WithMessage("O campo 'identificador' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.identificador));
        }
    }
}
