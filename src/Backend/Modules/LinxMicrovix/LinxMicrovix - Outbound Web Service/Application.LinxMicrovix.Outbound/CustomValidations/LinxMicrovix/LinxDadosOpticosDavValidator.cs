using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxDadosOpticosDavValidator : AbstractValidator<LinxDadosOpticosDav>
    {
        public LinxDadosOpticosDavValidator()
        {
            RuleFor(x => x.id_vendas_pos_produtos).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.id_vendas_pos_produtos));
            RuleFor(x => x.cod_produto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_produto));
            RuleFor(x => x.dav).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.dav));
            RuleFor(x => x.tipo_lente).MaximumLength(20).WithMessage("O campo 'tipo_lente' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.tipo_lente));
            RuleFor(x => x.od_oe).MaximumLength(2).WithMessage("O campo 'od_oe' deve ter no máximo 2 caracteres.").When(x => !string.IsNullOrEmpty(x.od_oe));
            RuleFor(x => x.lng_esferico_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_esferico_od));
            RuleFor(x => x.lng_cilindrico_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_cilindrico_od));
            RuleFor(x => x.lng_eixo_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_eixo_od));
            RuleFor(x => x.lng_esferico_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_esferico_oe));
            RuleFor(x => x.lng_cilindrico_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_cilindrico_oe));
            RuleFor(x => x.lng_eixo_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_eixo_oe));
            RuleFor(x => x.prt_esferico_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_esferico_od));
            RuleFor(x => x.prt_cilindrico_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_cilindrico_od));
            RuleFor(x => x.prt_eixo_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_eixo_od));
            RuleFor(x => x.prt_esferico_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_esferico_oe));
            RuleFor(x => x.prt_cilindrico_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_cilindrico_oe));
            RuleFor(x => x.prt_eixo_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_eixo_oe));
            RuleFor(x => x.cmpl_add_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.cmpl_add_od));
            RuleFor(x => x.base_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.base_od));
            RuleFor(x => x.diametro_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.diametro_od));
            RuleFor(x => x.cmpl_add_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.cmpl_add_oe));
            RuleFor(x => x.base_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.base_oe));
            RuleFor(x => x.diametro_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.diametro_oe));
            RuleFor(x => x.ponte).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.ponte));
            RuleFor(x => x.altura).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.altura));
            RuleFor(x => x.aro).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.aro));
            RuleFor(x => x.diag_maior).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.diag_maior));
            RuleFor(x => x.dist_haste).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.dist_haste));
            RuleFor(x => x.dist_front).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.dist_front));
            RuleFor(x => x.tipo_armacao).MaximumLength(1).WithMessage("O campo 'tipo_armacao' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.tipo_armacao));
            RuleFor(x => x.curvatura).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.curvatura));
            RuleFor(x => x.id_produtos_opticos_tipo_aro).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_produtos_opticos_tipo_aro));
            RuleFor(x => x.id_produtos_opticos_formato_aro).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_produtos_opticos_formato_aro));
            RuleFor(x => x.lng_dpdpn_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_dpdpn_od));
            RuleFor(x => x.prt_dpdpn_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_dpdpn_od));
            RuleFor(x => x.lng_dpdpn_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_dpdpn_oe));
            RuleFor(x => x.prt_dpdpn_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_dpdpn_oe));
            RuleFor(x => x.lng_altura_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_altura_od));
            RuleFor(x => x.prt_altura_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_altura_od));
            RuleFor(x => x.lng_altura_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_altura_oe));
            RuleFor(x => x.prt_altura_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_altura_oe));
            RuleFor(x => x.calibre).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.calibre));
            RuleFor(x => x.dp_montagem).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.dp_montagem));
            RuleFor(x => x.paciente).MaximumLength(100).WithMessage("O campo 'paciente' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.paciente));
            RuleFor(x => x.endereco_paciente).MaximumLength(100).WithMessage("O campo 'endereco_paciente' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.endereco_paciente));
            RuleFor(x => x.medico_especialista).MaximumLength(100).WithMessage("O campo 'medico_especialista' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.medico_especialista));
            RuleFor(x => x.endereco_medico_especialista).MaximumLength(100).WithMessage("O campo 'endereco_medico_especialista' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.endereco_medico_especialista));
            RuleFor(x => x.refracionista_optico).MaximumLength(100).WithMessage("O campo 'refracionista_optico' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.refracionista_optico));
            RuleFor(x => x.data_receita).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_receita));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.id_ordservprod).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_ordservprod));
            RuleFor(x => x.id_vendas_pos_produtos_tmp).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_vendas_pos_produtos_tmp));
            RuleFor(x => x.id_vendas_pos).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.id_vendas_pos));
            RuleFor(x => x.cancelado).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.cancelado));
            RuleFor(x => x.previsao_entrega).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.previsao_entrega));
            RuleFor(x => x.numero_ficha).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.numero_ficha));
            RuleFor(x => x.id_vendas_pos_produtos_campos_adicionais_tmp).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_vendas_pos_produtos_campos_adicionais_tmp));
            RuleFor(x => x.id_link_pagamento_linx_pay_hub).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_link_pagamento_linx_pay_hub));
            RuleFor(x => x.codigo_gerencial).MaximumLength(20).WithMessage("O campo 'codigo_gerencial' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_gerencial));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.origem));
            RuleFor(x => x.acoes_promocionais_desconto).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.acoes_promocionais_desconto));
            RuleFor(x => x.desconto).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.desconto));
            RuleFor(x => x.acrescimo).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.acrescimo));
            RuleFor(x => x.frete).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.frete));
            RuleFor(x => x.descontos_impostos).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.descontos_impostos));
            RuleFor(x => x.acrescimos_impostos).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.acrescimos_impostos));
            RuleFor(x => x.preco_unitario).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.preco_unitario));
            RuleFor(x => x.quantidade).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.quantidade));
            RuleFor(x => x.desconto_item).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.desconto_item));
            RuleFor(x => x.acrescimo_item_valor).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.acrescimo_item_valor));
            RuleFor(x => x.desconto_item_valor).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.desconto_item_valor));
        }
    }
}
