using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Optics;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Optics
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script067_Create_LinxMicrovix_Optics_Tables
    /// </summary>
    public class LinxPosicaoOsRamoOpticoValidator : AbstractValidator<LinxPosicaoOsRamoOptico>
    {
        public LinxPosicaoOsRamoOpticoValidator()
        {
            RuleFor(x => x.id_posicao_os_ramo_optico)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_posicao_os_ramo_optico' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(50)
                .WithMessage("O campo 'descricao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.codigo_status_padrao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_status_padrao));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
