using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoRemessasAcertosValidator : AbstractValidator<LinxMovimentoRemessasAcertos>
    {
        public LinxMovimentoRemessasAcertosValidator()
        {
            RuleFor(x => x.id_remessas_acertos).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessas_acertos));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.id_remessas).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessas));
            RuleFor(x => x.identificador_venda).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_venda' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_venda));
            RuleFor(x => x.identificador_retorno).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_retorno' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_retorno));
            RuleFor(x => x.identificador_devolucao).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_devolucao' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_devolucao));
            RuleFor(x => x.data).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data));
            RuleFor(x => x.id_vendas_pos).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.id_vendas_pos));
            RuleFor(x => x.excluido).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.excluido));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}