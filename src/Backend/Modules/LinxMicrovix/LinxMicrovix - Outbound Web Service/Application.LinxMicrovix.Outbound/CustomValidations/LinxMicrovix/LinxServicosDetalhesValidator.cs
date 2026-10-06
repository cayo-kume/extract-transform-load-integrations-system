using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxServicosDetalhesValidator : AbstractValidator<LinxServicosDetalhes>
    {
        public LinxServicosDetalhesValidator()
        {
        }
    }
}
