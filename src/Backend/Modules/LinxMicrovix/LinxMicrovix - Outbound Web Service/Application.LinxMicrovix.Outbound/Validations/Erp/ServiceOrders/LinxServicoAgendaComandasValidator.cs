using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.ServiceOrders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.ServiceOrders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script064_Create_LinxMicrovix_Service_Orders_Tables
    /// </summary>
    public class LinxServicoAgendaComandasValidator : AbstractValidator<LinxServicoAgendaComandas>
    {
        public LinxServicoAgendaComandasValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_comanda)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_comanda' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cliente));

            RuleFor(x => x.data)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data));

            RuleFor(x => x.noiva)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.noiva));

            RuleFor(x => x.excluida)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluida));

            RuleFor(x => x.fechada)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.fechada));

            RuleFor(x => x.observacoes)
                .MaximumLength(1000)
                .WithMessage("O campo 'observacoes' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observacoes));

            RuleFor(x => x.tipo_desconto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tipo_desconto));

            RuleFor(x => x.valor_desconto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_desconto));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
