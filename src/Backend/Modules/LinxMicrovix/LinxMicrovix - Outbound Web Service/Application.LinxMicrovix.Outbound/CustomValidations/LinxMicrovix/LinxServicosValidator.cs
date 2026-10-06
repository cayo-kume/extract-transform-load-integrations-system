using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxServicosValidator : AbstractValidator<LinxServicos>
    {
        public LinxServicosValidator()
        {
            RuleFor(x => x.id_setor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_setor));
            RuleFor(x => x.cod_servico).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_servico));
            RuleFor(x => x.nome).MaximumLength(250).WithMessage("O campo 'nome' deve ter no máximo 250 caracteres.").When(x => !string.IsNullOrEmpty(x.nome));
            RuleFor(x => x.desc_setor).MaximumLength(30).WithMessage("O campo 'desc_setor' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_setor));
            RuleFor(x => x.id_linha).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_linha));
            RuleFor(x => x.desc_linha).MaximumLength(30).WithMessage("O campo 'desc_linha' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_linha));
            RuleFor(x => x.id_marca).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_marca));
            RuleFor(x => x.desc_marca).MaximumLength(30).WithMessage("O campo 'desc_marca' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_marca));
            RuleFor(x => x.dt_update).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.dt_update));
            RuleFor(x => x.operacao_servico).MaximumLength(1).WithMessage("O campo 'operacao_servico' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.operacao_servico));
            RuleFor(x => x.servico_km).MaximumLength(1).WithMessage("O campo 'servico_km' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.servico_km));
            RuleFor(x => x.desativado).MaximumLength(1).WithMessage("O campo 'desativado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.desativado));
            RuleFor(x => x.cod_lc11603).MaximumLength(4).WithMessage("O campo 'cod_lc11603' deve ter no máximo 4 caracteres.").When(x => !string.IsNullOrEmpty(x.cod_lc11603));
            RuleFor(x => x.codigo_nbs).MaximumLength(20).WithMessage("O campo 'codigo_nbs' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_nbs));
            RuleFor(x => x.dt_inclusao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.dt_inclusao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.codigo_ws).MaximumLength(50).WithMessage("O campo 'codigo_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_ws));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
