using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxNfseValidator : AbstractValidator<LinxNfse>
    {
        public LinxNfseValidator()
        {
            RuleFor(x => x.id_nfse).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_nfse));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.documento).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.documento));
            RuleFor(x => x.codigo_verificacao).MaximumLength(50).WithMessage("O campo 'codigo_verificacao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_verificacao));
            RuleFor(x => x.id_nfse_situacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_nfse_situacao));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.dt_insert).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.dt_insert));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
