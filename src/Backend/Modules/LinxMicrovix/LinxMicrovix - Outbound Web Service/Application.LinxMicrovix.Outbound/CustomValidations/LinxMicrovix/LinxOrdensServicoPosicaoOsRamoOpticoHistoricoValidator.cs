using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxOrdensServicoPosicaoOsRamoOpticoHistoricoValidator : AbstractValidator<LinxOrdensServicoPosicaoOsRamoOpticoHistorico>
    {
        public LinxOrdensServicoPosicaoOsRamoOpticoHistoricoValidator()
        {
            RuleFor(x => x.id_ordens_servico_posicao_os_ramo_optico_historico).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_ordens_servico_posicao_os_ramo_optico_historico));
            RuleFor(x => x.numero_os).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.numero_os));
            RuleFor(x => x.usuario).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.usuario));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.id_posicao_os_ramo_optico).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_posicao_os_ramo_optico));
            RuleFor(x => x.data).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data));
            RuleFor(x => x.observacao).MaximumLength(200).WithMessage("O campo 'observacao' deve ter no máximo 200 caracteres.").When(x => !string.IsNullOrEmpty(x.observacao));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
