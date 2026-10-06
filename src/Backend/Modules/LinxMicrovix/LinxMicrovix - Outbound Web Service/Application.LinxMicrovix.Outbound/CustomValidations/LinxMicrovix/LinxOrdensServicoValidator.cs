using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxOrdensServicoValidator : AbstractValidator<LinxOrdensServico>
    {
        public LinxOrdensServicoValidator()
        {
            RuleFor(x => x.numero_os).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.numero_os));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.data_os).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_os));
            RuleFor(x => x.data_envio_laboratorio).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_envio_laboratorio));
            RuleFor(x => x.cancelado).MaximumLength(1).WithMessage("O campo 'cancelado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.cancelado));
            RuleFor(x => x.id_laboratorio).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_laboratorio));
            RuleFor(x => x.id_posicao_os_ramo_optico).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_posicao_os_ramo_optico));
            RuleFor(x => x.compartilhar_hub_laboratorios).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.compartilhar_hub_laboratorios));
            RuleFor(x => x.cod_cliente_os).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_cliente_os));
            RuleFor(x => x.cod_vendedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_vendedor));
            RuleFor(x => x.numero_sequencial_antecipacao_financeira).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.numero_sequencial_antecipacao_financeira));
            RuleFor(x => x.chave_nfe_laboratorio).MaximumLength(44).WithMessage("O campo 'chave_nfe_laboratorio' deve ter no máximo 44 caracteres.").When(x => !string.IsNullOrEmpty(x.chave_nfe_laboratorio));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
