using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxProdutosOpticosFormatoAroValidator : AbstractValidator<LinxProdutosOpticosFormatoAro>
    {
        public LinxProdutosOpticosFormatoAroValidator()
        {
            RuleFor(x => x.id_formato_aro).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_formato_aro));
            RuleFor(x => x.codigo).MaximumLength(16).WithMessage("O campo 'codigo' deve ter no máximo 16 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo));
            RuleFor(x => x.descricao).MaximumLength(60).WithMessage("O campo 'descricao' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}