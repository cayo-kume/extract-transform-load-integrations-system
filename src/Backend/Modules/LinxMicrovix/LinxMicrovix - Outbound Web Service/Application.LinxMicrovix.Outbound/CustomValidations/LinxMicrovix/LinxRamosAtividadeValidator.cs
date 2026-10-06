using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxRamosAtividadeValidator : AbstractValidator<LinxRamosAtividade>
    {
        public LinxRamosAtividadeValidator()
        {
            RuleFor(x => x.id_ramo_atividade).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_ramo_atividade));
            RuleFor(x => x.nome_ramo_atividade).MaximumLength(50).WithMessage("O campo 'nome_ramo_atividade' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_ramo_atividade));
            RuleFor(x => x.codigo_ramo_atividade).MaximumLength(12).WithMessage("O campo 'codigo_ramo_atividade' deve ter no máximo 12 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_ramo_atividade));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
