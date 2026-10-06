using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxConfiguracoesTributariasValidator : AbstractValidator<LinxConfiguracoesTributarias>
    {
        public LinxConfiguracoesTributariasValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.id_config_tributaria).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_config_tributaria));
            RuleFor(x => x.desc_config_tributaria).MaximumLength(100).WithMessage("O campo 'desc_config_tributaria' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.desc_config_tributaria));
            RuleFor(x => x.sigla_config_tributaria).MaximumLength(50).WithMessage("O campo 'sigla_config_tributaria' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.sigla_config_tributaria));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.ativa).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ativa));
            RuleFor(x => x.uf).MaximumLength(2).WithMessage("O campo 'uf' deve ter no máximo 2 caracteres.").When(x => !string.IsNullOrEmpty(x.uf));
            RuleFor(x => x.sistema_tributacao).MaximumLength(1).WithMessage("O campo 'sistema_tributacao' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.sistema_tributacao));
            RuleFor(x => x.tipo_atividade).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.tipo_atividade));
            RuleFor(x => x.id_origem_mercadoria).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_origem_mercadoria));
            RuleFor(x => x.utiliza_uso_consumo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.utiliza_uso_consumo));
            RuleFor(x => x.id_classificacao_cest_produto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_classificacao_cest_produto));
            RuleFor(x => x.codigo_ws).MaximumLength(50).WithMessage("O campo 'codigo_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_ws));
        }
    }
}
