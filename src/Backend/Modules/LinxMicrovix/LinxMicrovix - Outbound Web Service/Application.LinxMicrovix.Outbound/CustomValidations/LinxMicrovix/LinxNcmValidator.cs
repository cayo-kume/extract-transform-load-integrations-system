using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxNcmValidator : AbstractValidator<LinxNcm>
    {
        public LinxNcmValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.codigo).MaximumLength(20).WithMessage("O campo 'codigo' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo));
            RuleFor(x => x.descricao).MaximumLength(60).WithMessage("O campo 'descricao' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.codigo_genero).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.codigo_genero));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.id_ncm).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_ncm));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
