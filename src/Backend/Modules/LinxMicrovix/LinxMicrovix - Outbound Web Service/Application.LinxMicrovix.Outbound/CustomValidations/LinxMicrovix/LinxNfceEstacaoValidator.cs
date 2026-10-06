using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxNfceEstacaoValidator : AbstractValidator<LinxNfceEstacao>
    {
        public LinxNfceEstacaoValidator()
        {
            RuleFor(x => x.id_nfce_estacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_nfce_estacao));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.descricao).MaximumLength(50).WithMessage("O campo 'descricao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.numero_pdv_tef).MaximumLength(20).WithMessage("O campo 'numero_pdv_tef' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.numero_pdv_tef));
            RuleFor(x => x.ativo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
