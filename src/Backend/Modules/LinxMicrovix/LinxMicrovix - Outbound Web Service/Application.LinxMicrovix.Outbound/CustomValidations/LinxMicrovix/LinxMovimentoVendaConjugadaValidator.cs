using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoVendaConjugadaValidator : AbstractValidator<LinxMovimentoVendaConjugada>
    {
        public LinxMovimentoVendaConjugadaValidator()
        {
            RuleFor(x => x.identificador_produto).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_produto' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_produto));
            RuleFor(x => x.identificador_servico).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_servico' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_servico));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
