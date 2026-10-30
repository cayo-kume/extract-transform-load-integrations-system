using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Customers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script028_Create_LinxMicrovix_Customers_Tables
    /// </summary>
    public class LinxRamosAtividadeValidator : AbstractValidator<LinxRamosAtividade>
    {
        public LinxRamosAtividadeValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_ramo_atividade)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_ramo_atividade' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_ramo_atividade)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_ramo_atividade' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_ramo_atividade));

            RuleFor(x => x.codigo_ramo_atividade)
                .MaximumLength(12)
                .WithMessage("O campo 'codigo_ramo_atividade' deve ter no máximo 12 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_ramo_atividade));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
