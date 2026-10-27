using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.ServiceOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script064_Create_LinxMicrovix_Service_Orders_Tables
    /// </summary>
    public class LinxServicoAgendaEventosValidator : AbstractValidator<LinxServicoAgendaEventos>
    {
        public LinxServicoAgendaEventosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_evento)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_evento' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_espaco)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_espaco));

            RuleFor(x => x.data_evento)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_evento));

            RuleFor(x => x.minutos_duracao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.minutos_duracao));

            RuleFor(x => x.tipo_agendamento)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tipo_agendamento));

            RuleFor(x => x.id_cliente_servico)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cliente_servico));

            RuleFor(x => x.id_profissional)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_profissional));

            RuleFor(x => x.situacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.situacao));

            RuleFor(x => x.data_situacao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_situacao));

            RuleFor(x => x.observacao)
                .MaximumLength(1000)
                .WithMessage("O campo 'observacao' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observacao));

            RuleFor(x => x.data_chegada)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_chegada));

            RuleFor(x => x.data_execucao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_execucao));

            RuleFor(x => x.recorrente)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.recorrente));

            RuleFor(x => x.id_comanda)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_comanda));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
