using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxFidelidadeValidator : AbstractValidator<LinxFidelidade>
    {
        public LinxFidelidadeValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.id_fidelidade_parceiro_log).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_fidelidade_parceiro_log));
            RuleFor(x => x.data_transacao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_transacao));
            RuleFor(x => x.operacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.operacao));
            RuleFor(x => x.aprovado_barramento).MaximumLength(4).WithMessage("O campo 'aprovado_barramento' deve ter no máximo 4 caracteres.").When(x => !string.IsNullOrEmpty(x.aprovado_barramento));
            RuleFor(x => x.valor_monetario).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_monetario));
            RuleFor(x => x.numero_cartao).MaximumLength(100).WithMessage("O campo 'numero_cartao' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.numero_cartao));
            RuleFor(x => x.identificador_movimento).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_movimento' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_movimento));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
