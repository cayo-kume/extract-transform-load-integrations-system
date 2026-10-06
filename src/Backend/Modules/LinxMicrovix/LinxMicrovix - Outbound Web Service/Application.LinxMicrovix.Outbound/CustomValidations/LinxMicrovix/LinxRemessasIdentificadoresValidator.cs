using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxRemessasIdentificadoresValidator : AbstractValidator<LinxRemessasIdentificadores>
    {
        public LinxRemessasIdentificadoresValidator()
        {
            RuleFor(x => x.identificador_venda).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_venda' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_venda));
            RuleFor(x => x.identificador_remessa).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_remessa' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_remessa));
            RuleFor(x => x.id_remessas).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessas));
            RuleFor(x => x.id_remessas_acertos).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessas_acertos));
            RuleFor(x => x.transacao_acerto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao_acerto));
            RuleFor(x => x.qtde_total_acerto).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_total_acerto));
            RuleFor(x => x.identificador_devolucao).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_devolucao' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_devolucao));
            RuleFor(x => x.transacao_remessa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao_remessa));
            RuleFor(x => x.id_remessa_operacoes).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_remessa_operacoes));
        }
    }
}
