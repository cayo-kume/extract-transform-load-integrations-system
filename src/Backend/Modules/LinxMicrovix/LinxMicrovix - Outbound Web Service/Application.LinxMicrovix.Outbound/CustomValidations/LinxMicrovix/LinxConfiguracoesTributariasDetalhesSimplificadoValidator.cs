using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxConfiguracoesTributariasDetalhesSimplificadoValidator : AbstractValidator<LinxConfiguracoesTributariasDetalhesSimplificado>
    {
        public LinxConfiguracoesTributariasDetalhesSimplificadoValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.id_config_tributaria_detalhe).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_config_tributaria_detalhe));
            RuleFor(x => x.id_config_tributaria).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_config_tributaria));
            RuleFor(x => x.cod_natureza_operacao).MaximumLength(10).WithMessage("O campo 'cod_natureza_operacao' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.cod_natureza_operacao));
            RuleFor(x => x.id_classe_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_classe_fiscal));
            RuleFor(x => x.id_cst_icms_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_cst_icms_fiscal));
            RuleFor(x => x.id_csosn_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_csosn_fiscal));
            RuleFor(x => x.id_cfop_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_cfop_fiscal));
            RuleFor(x => x.ipi_credito).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ipi_credito));
            RuleFor(x => x.icms_credito).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.icms_credito));
            RuleFor(x => x.aliq_icms).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.aliq_icms));
            RuleFor(x => x.perc_reducao_icms).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.perc_reducao_icms));
            RuleFor(x => x.aliquota_st).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.aliquota_st));
            RuleFor(x => x.margem_st).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.margem_st));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
