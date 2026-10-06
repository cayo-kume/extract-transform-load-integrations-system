using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoExtensaoValidator : AbstractValidator<LinxMovimentoExtensao>
    {
        public LinxMovimentoExtensaoValidator()
        {
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.transacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao));
            RuleFor(x => x.base_fcp_st).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.base_fcp_st));
            RuleFor(x => x.valor_fcp_st).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_fcp_st));
            RuleFor(x => x.aliq_fcp_st).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.aliq_fcp_st));
            RuleFor(x => x.base_icms_fcp_st).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.base_icms_fcp_st));
            RuleFor(x => x.valor_icms_fcp_st).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_icms_fcp_st));
            RuleFor(x => x.base_icms_fcp_st_retido).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.base_icms_fcp_st_retido));
            RuleFor(x => x.valor_icms_fcp_st_retido).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_icms_fcp_st_retido));
            RuleFor(x => x.base_icms_fcp_st_antecipado).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.base_icms_fcp_st_antecipado));
            RuleFor(x => x.valor_icms_fcp_st_antecipado).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_icms_fcp_st_antecipado));
            RuleFor(x => x.aliquota_icms_fcp_st_antecipado).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.aliquota_icms_fcp_st_antecipado));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.valor_iss).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_iss));
            RuleFor(x => x.tipo_tributacao_iss).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.tipo_tributacao_iss));
            RuleFor(x => x.icms_fcp_aliquota).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_fcp_aliquota));
            RuleFor(x => x.icms_fcp_base_item).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_fcp_base_item));
            RuleFor(x => x.icms_fcp_valor_item).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_fcp_valor_item));
            RuleFor(x => x.icms_base_partilha).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_base_partilha));
            RuleFor(x => x.aliq_difal_interna_uf_destinatario).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.aliq_difal_interna_uf_destinatario));
            RuleFor(x => x.aliq_difal_interestadual_uf_envolvidas).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.aliq_difal_interestadual_uf_envolvidas));
            RuleFor(x => x.icms_item_perc_partilha_destino).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_item_perc_partilha_destino));
            RuleFor(x => x.icms_item_perc_partilha_origem).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_item_perc_partilha_origem));
            RuleFor(x => x.codigo_pacote).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigo_pacote));
            RuleFor(x => x.codigo_itens_associados).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigo_itens_associados));
            RuleFor(x => x.codigo_kit).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigo_kit));
            RuleFor(x => x.id_motivo_desconto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_motivo_desconto));
            RuleFor(x => x.icms_st_antecipado_base_item).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_st_antecipado_base_item));
            RuleFor(x => x.icms_suportado_valor_item).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_suportado_valor_item));
            RuleFor(x => x.icms_suportado_valor_unitario).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_suportado_valor_unitario));
            RuleFor(x => x.icms_st_pago_base).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_st_pago_base));
            RuleFor(x => x.icms_st_pago_valor).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_st_pago_valor));
            RuleFor(x => x.icms_st_pago_aliq).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_st_pago_aliq));
            RuleFor(x => x.icms_para_st_pago_valor).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.icms_para_st_pago_valor));
        }
    }
}