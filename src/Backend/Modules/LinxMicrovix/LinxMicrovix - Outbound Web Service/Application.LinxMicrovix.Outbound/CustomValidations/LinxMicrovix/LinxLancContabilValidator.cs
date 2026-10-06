using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxLancContabilValidator : AbstractValidator<LinxLancContabil>
    {
        public LinxLancContabilValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.cod_lanc).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_lanc));
            RuleFor(x => x.centro_custo).MaximumLength(50).WithMessage("O campo 'centro_custo' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.centro_custo));
            RuleFor(x => x.ind_conta).MaximumLength(150).WithMessage("O campo 'ind_conta' deve ter no máximo 150 caracteres.").When(x => !string.IsNullOrEmpty(x.ind_conta));
            RuleFor(x => x.cod_conta).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_conta));
            RuleFor(x => x.nome_conta).MaximumLength(50).WithMessage("O campo 'nome_conta' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_conta));
            RuleFor(x => x.valor_conta).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_conta));
            RuleFor(x => x.cred_deb).MaximumLength(1).WithMessage("O campo 'cred_deb' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.cred_deb));
            RuleFor(x => x.data_lanc).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_lanc));
            RuleFor(x => x.compl_conta).MaximumLength(500).WithMessage("O campo 'compl_conta' deve ter no máximo 500 caracteres.").When(x => !string.IsNullOrEmpty(x.compl_conta));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.cod_historico).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_historico));
            RuleFor(x => x.desc_historico).MaximumLength(50).WithMessage("O campo 'desc_historico' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_historico));
            RuleFor(x => x.data_compensacao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_compensacao));
            RuleFor(x => x.fatura_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.fatura_origem));
            RuleFor(x => x.efetivado).MaximumLength(1).WithMessage("O campo 'efetivado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.efetivado));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.id_lanc).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.id_lanc));
            RuleFor(x => x.cancelado).MaximumLength(1).WithMessage("O campo 'cancelado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.cancelado));
        }
    }
}