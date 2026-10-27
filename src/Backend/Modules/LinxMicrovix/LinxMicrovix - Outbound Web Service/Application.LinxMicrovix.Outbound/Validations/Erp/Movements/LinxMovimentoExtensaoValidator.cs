using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Movements
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script040_Create_LinxMicrovix_Movements_Tables
    /// </summary>
    public class LinxMovimentoExtensaoValidator : AbstractValidator<LinxMovimentoExtensao>
    {
        public LinxMovimentoExtensaoValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.transacao)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'transacao' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.base_fcp_st)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.base_fcp_st));

            RuleFor(x => x.valor_fcp_st)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_fcp_st));

            RuleFor(x => x.aliq_fcp_st)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliq_fcp_st));

            RuleFor(x => x.base_icms_fcp_st)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.base_icms_fcp_st));

            RuleFor(x => x.valor_icms_fcp_st)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_icms_fcp_st));

            RuleFor(x => x.base_icms_fcp_st_retido)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.base_icms_fcp_st_retido));

            RuleFor(x => x.valor_icms_fcp_st_retido)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_icms_fcp_st_retido));

            RuleFor(x => x.base_icms_fcp_st_antecipado)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.base_icms_fcp_st_antecipado));

            RuleFor(x => x.valor_icms_fcp_st_antecipado)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_icms_fcp_st_antecipado));

            RuleFor(x => x.aliquota_icms_fcp_st_antecipado)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliquota_icms_fcp_st_antecipado));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.valor_iss)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_iss));

            RuleFor(x => x.tipo_tributacao_iss)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.tipo_tributacao_iss));

            RuleFor(x => x.icms_fcp_aliquota)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_fcp_aliquota));

            RuleFor(x => x.icms_fcp_base_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_fcp_base_item));

            RuleFor(x => x.icms_fcp_valor_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_fcp_valor_item));

            RuleFor(x => x.icms_base_partilha)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_base_partilha));

            RuleFor(x => x.aliq_difal_interna_uf_destinatario)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliq_difal_interna_uf_destinatario));

            RuleFor(x => x.aliq_difal_interestadual_uf_envolvidas)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliq_difal_interestadual_uf_envolvidas));

            RuleFor(x => x.icms_item_perc_partilha_destino)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_item_perc_partilha_destino));

            RuleFor(x => x.icms_item_perc_partilha_origem)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_item_perc_partilha_origem));

            RuleFor(x => x.codigo_pacote)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigo_pacote));

            RuleFor(x => x.codigo_itens_associados)
                .MaximumLength(1000)
                .WithMessage("O campo 'codigo_itens_associados' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_itens_associados));

            RuleFor(x => x.codigo_kit)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigo_kit));

            RuleFor(x => x.id_motivo_desconto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_motivo_desconto));

            RuleFor(x => x.icms_st_antecipado_base_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_st_antecipado_base_item));

            RuleFor(x => x.icms_suportado_valor_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_suportado_valor_item));

            RuleFor(x => x.icms_suportado_valor_unitario)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_suportado_valor_unitario));

            RuleFor(x => x.icms_st_pago_base)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_st_pago_base));

            RuleFor(x => x.icms_st_pago_valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_st_pago_valor));

            RuleFor(x => x.icms_st_pago_aliq)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_st_pago_aliq));

            RuleFor(x => x.icms_para_st_pago_valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.icms_para_st_pago_valor));

            RuleFor(x => x.cod_cliente_indicador)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_indicador));

            RuleFor(x => x.motivo_desconto_observacao)
                .MaximumLength(500)
                .WithMessage("O campo 'motivo_desconto_observacao' deve ter no máximo 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.motivo_desconto_observacao));

            RuleFor(x => x.codigo_classificacao_tributaria)
                .MaximumLength(6)
                .WithMessage("O campo 'codigo_classificacao_tributaria' deve ter no máximo 6 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_classificacao_tributaria));

            RuleFor(x => x.codigo_cst_cbs_ibs)
                .MaximumLength(3)
                .WithMessage("O campo 'codigo_cst_cbs_ibs' deve ter no máximo 3 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_cst_cbs_ibs));

            RuleFor(x => x.base_cbs_ibs_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.base_cbs_ibs_item));

            RuleFor(x => x.aliquota_cbs)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliquota_cbs));

            RuleFor(x => x.valor_cbs_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_cbs_item));

            RuleFor(x => x.percentual_reducao_cbs)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentual_reducao_cbs));

            RuleFor(x => x.aliquota_efetiva_cbs)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliquota_efetiva_cbs));

            RuleFor(x => x.aliquota_ibs_uf)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliquota_ibs_uf));

            RuleFor(x => x.valor_ibs_uf_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_ibs_uf_item));

            RuleFor(x => x.percentual_reducao_ibs_uf)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentual_reducao_ibs_uf));

            RuleFor(x => x.aliquota_efetiva_ibs_uf)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliquota_efetiva_ibs_uf));

            RuleFor(x => x.aliquota_ibs_municipio)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliquota_ibs_municipio));

            RuleFor(x => x.valor_ibs_municipio_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_ibs_municipio_item));

            RuleFor(x => x.percentual_reducao_ibs_municipio)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.percentual_reducao_ibs_municipio));

            RuleFor(x => x.aliquota_efetiva_ibs_municipio)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliquota_efetiva_ibs_municipio));

            RuleFor(x => x.valor_ibs_total_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_ibs_total_item));

            RuleFor(x => x.custo_aquisicao_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.custo_aquisicao_item));
        }
    }
}
