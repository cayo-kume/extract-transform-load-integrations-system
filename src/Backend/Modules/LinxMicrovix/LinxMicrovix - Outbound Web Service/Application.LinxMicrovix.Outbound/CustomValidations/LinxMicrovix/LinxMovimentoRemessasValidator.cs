using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoRemessasValidator : AbstractValidator<LinxMovimentoRemessas>
    {
        public LinxMovimentoRemessasValidator()
        {
            RuleFor(x => x.id_remessas).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessas));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.id_tipo).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_tipo));
            RuleFor(x => x.identificador_remessa).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_remessa' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_remessa));
            RuleFor(x => x.status).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.status));
            RuleFor(x => x.status_descricao).MaximumLength(35).WithMessage("O campo 'status_descricao' deve ter no máximo 35 caracteres.").When(x => !string.IsNullOrEmpty(x.status_descricao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}