using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Optics
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script067_Create_LinxMicrovix_Optics_Tables
    /// </summary>
    public class LinxOrdensServicoPosicaoOsRamoOpticoHistoricoValidator : AbstractValidator<LinxOrdensServicoPosicaoOsRamoOpticoHistorico>
    {
        public LinxOrdensServicoPosicaoOsRamoOpticoHistoricoValidator()
        {
            RuleFor(x => x.id_ordens_servico_posicao_os_ramo_optico_historico)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_ordens_servico_posicao_os_ramo_optico_historico' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.numero_os)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_os));

            RuleFor(x => x.usuario)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.usuario));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.id_posicao_os_ramo_optico)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_posicao_os_ramo_optico));

            RuleFor(x => x.data)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data));

            RuleFor(x => x.observacao)
                .MaximumLength(200)
                .WithMessage("O campo 'observacao' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observacao));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
