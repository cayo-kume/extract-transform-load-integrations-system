using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoOrigemDevolucoesValidator : AbstractValidator<LinxMovimentoOrigemDevolucoes>
    {
        public LinxMovimentoOrigemDevolucoesValidator()
        {
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.nota_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.nota_origem));
            RuleFor(x => x.ecf_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.ecf_origem));
            RuleFor(x => x.data_origem).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_origem));
            RuleFor(x => x.serie_origem).MaximumLength(10).WithMessage("O campo 'serie_origem' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.serie_origem));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}