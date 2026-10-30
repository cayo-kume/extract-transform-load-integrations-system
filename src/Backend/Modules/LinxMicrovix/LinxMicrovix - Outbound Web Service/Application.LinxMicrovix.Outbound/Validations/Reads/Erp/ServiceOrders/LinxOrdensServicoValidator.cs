using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.ServiceOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script064_Create_LinxMicrovix_Service_Orders_Tables
    /// </summary>
    public class LinxOrdensServicoValidator : AbstractValidator<LinxOrdensServico>
    {
        public LinxOrdensServicoValidator()
        {
            RuleFor(x => x.numero_os)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'numero_os' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.data_os)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_os));

            RuleFor(x => x.data_envio_laboratorio)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_envio_laboratorio));

            RuleFor(x => x.cancelado)
                .MaximumLength(1)
                .WithMessage("O campo 'cancelado' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cancelado));

            RuleFor(x => x.id_laboratorio)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_laboratorio));

            RuleFor(x => x.id_posicao_os_ramo_optico)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_posicao_os_ramo_optico));

            RuleFor(x => x.compartilhar_hub_laboratorios)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.compartilhar_hub_laboratorios));

            RuleFor(x => x.cod_cliente_os)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_os));

            RuleFor(x => x.cod_vendedor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_vendedor));

            RuleFor(x => x.numero_sequencial_antecipacao_financeira)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_sequencial_antecipacao_financeira));

            RuleFor(x => x.chave_nfe_laboratorio)
                .MaximumLength(44)
                .WithMessage("O campo 'chave_nfe_laboratorio' deve ter no máximo 44 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chave_nfe_laboratorio));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.previsao_entrega_os)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.previsao_entrega_os));
        }
    }
}
