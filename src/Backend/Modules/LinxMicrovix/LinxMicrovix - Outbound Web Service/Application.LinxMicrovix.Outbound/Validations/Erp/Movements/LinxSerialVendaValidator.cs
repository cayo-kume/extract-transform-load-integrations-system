using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Movements
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script040_Create_LinxMicrovix_Movements_Tables
    /// </summary>
    public class LinxSerialVendaValidator : AbstractValidator<LinxSerialVenda>
    {
        public LinxSerialVendaValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_serial_venda)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_serial_venda' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.transacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.serial)
                .MaximumLength(50)
                .WithMessage("O campo 'serial' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serial));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
