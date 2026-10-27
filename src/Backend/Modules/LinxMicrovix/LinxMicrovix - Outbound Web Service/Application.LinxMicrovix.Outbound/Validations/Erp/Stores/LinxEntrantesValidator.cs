using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Stores
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script004_Create_LinxMicrovix_Stores_Tables
    /// </summary>
    public class LinxEntrantesValidator : AbstractValidator<LinxEntrantes>
    {
        public LinxEntrantesValidator()
        {
            RuleFor(x => x.id_entrantes)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_entrantes' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.data_inicio)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_inicio));

            RuleFor(x => x.data_fim)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_fim));

            RuleFor(x => x.cnpj_empresa)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_empresa' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_empresa));

            RuleFor(x => x.entrantes)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.entrantes));

            RuleFor(x => x.portal)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'portal' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.timestamp)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'timestamp' é obrigatório.")
                .MustBeValidInt64();
        }
    }
}
