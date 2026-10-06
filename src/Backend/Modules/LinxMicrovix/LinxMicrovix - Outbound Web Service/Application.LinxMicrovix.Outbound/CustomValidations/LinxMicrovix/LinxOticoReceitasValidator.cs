using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxOticoReceitasValidator : AbstractValidator<LinxOticoReceitas>
    {
        public LinxOticoReceitasValidator()
        {
            RuleFor(x => x.id_otico_receitas).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_otico_receitas));
            RuleFor(x => x.cod_cliente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_cliente));
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.lng_esferico_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_esferico_od));
            RuleFor(x => x.lng_cilindrico_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_cilindrico_od));
            RuleFor(x => x.lng_eixo_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_eixo_od));
            RuleFor(x => x.lng_dpdpn_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_dpdpn_od));
            RuleFor(x => x.lng_altura_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_altura_od));
            RuleFor(x => x.prt_esferico_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_esferico_od));
            RuleFor(x => x.prt_cilindrico_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_cilindrico_od));
            RuleFor(x => x.prt_eixo_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_eixo_od));
            RuleFor(x => x.prt_dpdpn_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_dpdpn_od));
            RuleFor(x => x.lng_esferico_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_esferico_oe));
            RuleFor(x => x.lng_cilindrico_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_cilindrico_oe));
            RuleFor(x => x.lng_eixo_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_eixo_oe));
            RuleFor(x => x.lng_dpdpn_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_dpdpn_oe));
            RuleFor(x => x.lng_altura_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_altura_oe));
            RuleFor(x => x.prt_esferico_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_esferico_oe));
            RuleFor(x => x.prt_cilindrico_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_cilindrico_oe));
            RuleFor(x => x.prt_eixo_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_eixo_oe));
            RuleFor(x => x.prt_dpdpn_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_dpdpn_oe));
            RuleFor(x => x.lng_adicao_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_adicao_od));
            RuleFor(x => x.lng_adicao_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_adicao_oe));
            RuleFor(x => x.otico_lado_lente).MaximumLength(15).WithMessage("O campo 'otico_lado_lente' deve ter no máximo 15 caracteres.").When(x => !string.IsNullOrEmpty(x.otico_lado_lente));
            RuleFor(x => x.diametro_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.diametro_od));
            RuleFor(x => x.diametro_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.diametro_oe));
            RuleFor(x => x.ponte).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.ponte));
            RuleFor(x => x.altura).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.altura));
            RuleFor(x => x.aro).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.aro));
            RuleFor(x => x.diag_maior).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.diag_maior));
            RuleFor(x => x.tipo_armacao).MaximumLength(100).WithMessage("O campo 'tipo_armacao' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.tipo_armacao));
            RuleFor(x => x.observacao).MaximumLength(100).WithMessage("O campo 'observacao' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.observacao));
            RuleFor(x => x.paciente).MaximumLength(100).WithMessage("O campo 'paciente' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.paciente));
            RuleFor(x => x.cmpl_add_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.cmpl_add_od));
            RuleFor(x => x.cmpl_add_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.cmpl_add_oe));
            RuleFor(x => x.base_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.base_od));
            RuleFor(x => x.base_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.base_oe));
            RuleFor(x => x.dist_haste).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.dist_haste));
            RuleFor(x => x.dist_front).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.dist_front));
            RuleFor(x => x.curvatura).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.curvatura));
            RuleFor(x => x.lng_pris1_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_pris1_od));
            RuleFor(x => x.id_lng_pris1_od_desc).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_lng_pris1_od_desc));
            RuleFor(x => x.lng_pris1_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_pris1_oe));
            RuleFor(x => x.id_lng_pris1_oe_desc).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_lng_pris1_oe_desc));
            RuleFor(x => x.prt_pris1_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_pris1_od));
            RuleFor(x => x.id_prt_pris1_od_desc).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_prt_pris1_od_desc));
            RuleFor(x => x.prt_pris1_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_pris1_oe));
            RuleFor(x => x.id_prt_pris1_oe_desc).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_prt_pris1_oe_desc));
            RuleFor(x => x.lng_pris2_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_pris2_od));
            RuleFor(x => x.id_lng_pris2_od_desc).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_lng_pris2_od_desc));
            RuleFor(x => x.lng_pris2_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.lng_pris2_oe));
            RuleFor(x => x.id_lng_pris2_oe_desc).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_lng_pris2_oe_desc));
            RuleFor(x => x.prt_pris2_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_pris2_od));
            RuleFor(x => x.id_prt_pris2_od_desc).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_prt_pris2_od_desc));
            RuleFor(x => x.prt_pris2_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.prt_pris2_oe));
            RuleFor(x => x.id_prt_pris2_oe_desc).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_prt_pris2_oe_desc));
            RuleFor(x => x.dist_vertice).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.dist_vertice));
            RuleFor(x => x.inc_planto).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.inc_planto));
            RuleFor(x => x.ativ_princ).MaximumLength(40).WithMessage("O campo 'ativ_princ' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.ativ_princ));
            RuleFor(x => x.otico_area_privilegiada).MaximumLength(15).WithMessage("O campo 'otico_area_privilegiada' deve ter no máximo 15 caracteres.").When(x => !string.IsNullOrEmpty(x.otico_area_privilegiada));
            RuleFor(x => x.dist_visao).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.dist_visao));
            RuleFor(x => x.personalizada).MaximumLength(100).WithMessage("O campo 'personalizada' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.personalizada));
            RuleFor(x => x.pris_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.pris_od));
            RuleFor(x => x.pris_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.pris_oe));
            RuleFor(x => x.eixo_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.eixo_od));
            RuleFor(x => x.eixo_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.eixo_oe));
            RuleFor(x => x.borda_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.borda_od));
            RuleFor(x => x.borda_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.borda_oe));
            RuleFor(x => x.centro_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.centro_od));
            RuleFor(x => x.centro_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.centro_oe));
            RuleFor(x => x.furo_od).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.furo_od));
            RuleFor(x => x.furo_oe).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.furo_oe));
            RuleFor(x => x.format_corte).MaximumLength(40).WithMessage("O campo 'format_corte' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.format_corte));
            RuleFor(x => x.dist_leitura).MaximumLength(40).WithMessage("O campo 'dist_leitura' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.dist_leitura));
            RuleFor(x => x.tipo_lente).MaximumLength(100).WithMessage("O campo 'tipo_lente' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.tipo_lente));
            RuleFor(x => x.endereco_paciente).MaximumLength(100).WithMessage("O campo 'endereco_paciente' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.endereco_paciente));
            RuleFor(x => x.medico_especialista).MaximumLength(10).WithMessage("O campo 'medico_especialista' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.medico_especialista));
            RuleFor(x => x.endereco_medico_especialista).MaximumLength(100).WithMessage("O campo 'endereco_medico_especialista' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.endereco_medico_especialista));
            RuleFor(x => x.refracionista_optico).MaximumLength(100).WithMessage("O campo 'refracionista_optico' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.refracionista_optico));
            RuleFor(x => x.data_receita).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_receita));
            RuleFor(x => x.id_produtos_opticos_tipo_aro).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_produtos_opticos_tipo_aro));
            RuleFor(x => x.dp_montagem).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.dp_montagem));
            RuleFor(x => x.id_produtos_opticos_formato_aro).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_produtos_opticos_formato_aro));
            RuleFor(x => x.codigo_gerencial).MaximumLength(40).WithMessage("O campo 'codigo_gerencial' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_gerencial));
            //RuleFor(x => x.migracao_dados).MustBeValidBoolean().When(x => x.migracao_dados.HasValue);
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
