using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxNcmValidator : AbstractValidator<LinxNcm>
    {
        public LinxNcmValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.codigo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'codigo' é obrigatório.")
                .MaximumLength(20)
                .WithMessage("O campo 'codigo' deve ter no máximo 20 caracteres.");

            RuleFor(x => x.descricao)
                .MaximumLength(60)
                .WithMessage("O campo 'descricao' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.codigo_genero)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_genero));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.id_ncm)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_ncm));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
