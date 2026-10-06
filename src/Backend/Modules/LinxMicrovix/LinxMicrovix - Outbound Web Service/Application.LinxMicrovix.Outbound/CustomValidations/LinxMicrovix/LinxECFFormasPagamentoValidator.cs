using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxECFFormasPagamentoValidator : AbstractValidator<LinxECFFormasPagamento>
    {
        public LinxECFFormasPagamentoValidator()
        {
            RuleFor(x => x.id_empresa_ecf_formas_pgto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_empresa_ecf_formas_pgto));
            RuleFor(x => x.id_ecf).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_ecf));
            RuleFor(x => x.cod_forma_pgto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_forma_pgto));
            RuleFor(x => x.indice_forma).MaximumLength(53).WithMessage("O campo 'indice_forma' deve ter no máximo 53 caracteres.").When(x => !string.IsNullOrEmpty(x.indice_forma));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
