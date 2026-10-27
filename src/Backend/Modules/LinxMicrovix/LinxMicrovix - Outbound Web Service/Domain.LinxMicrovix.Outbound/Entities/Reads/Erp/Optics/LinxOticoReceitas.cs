using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Optics
{
    /// <summary>
    /// LinxOticoReceitas, tabela linxmicrovix.LinxOticoReceitas. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_otico_receitas (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxOticoReceitas
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_otico_receitas { get; private set; }
        public int? cod_cliente { get; private set; }
        public long? codigoproduto { get; private set; }
        public decimal? lng_esferico_od { get; private set; }
        public decimal? lng_cilindrico_od { get; private set; }
        public decimal? lng_eixo_od { get; private set; }
        public decimal? lng_dpdpn_od { get; private set; }
        public decimal? lng_altura_od { get; private set; }
        public decimal? prt_esferico_od { get; private set; }
        public decimal? prt_cilindrico_od { get; private set; }
        public decimal? prt_eixo_od { get; private set; }
        public decimal? prt_dpdpn_od { get; private set; }
        public decimal? lng_esferico_oe { get; private set; }
        public decimal? lng_cilindrico_oe { get; private set; }
        public decimal? lng_eixo_oe { get; private set; }
        public decimal? lng_dpdpn_oe { get; private set; }
        public decimal? lng_altura_oe { get; private set; }
        public decimal? prt_esferico_oe { get; private set; }
        public decimal? prt_cilindrico_oe { get; private set; }
        public decimal? prt_eixo_oe { get; private set; }
        public decimal? prt_dpdpn_oe { get; private set; }
        public decimal? lng_adicao_od { get; private set; }
        public decimal? lng_adicao_oe { get; private set; }
        public string? otico_lado_lente { get; private set; }
        public decimal? diametro_od { get; private set; }
        public decimal? diametro_oe { get; private set; }
        public decimal? ponte { get; private set; }
        public decimal? altura { get; private set; }
        public decimal? aro { get; private set; }
        public decimal? diag_maior { get; private set; }
        public string? tipo_armacao { get; private set; }
        public string? observacao { get; private set; }
        public string? paciente { get; private set; }
        public decimal? cmpl_add_od { get; private set; }
        public decimal? cmpl_add_oe { get; private set; }
        public decimal? base_od { get; private set; }
        public decimal? base_oe { get; private set; }
        public decimal? dist_haste { get; private set; }
        public decimal? dist_front { get; private set; }
        public decimal? curvatura { get; private set; }
        public decimal? lng_pris1_od { get; private set; }
        public int? id_lng_pris1_od_desc { get; private set; }
        public decimal? lng_pris1_oe { get; private set; }
        public int? id_lng_pris1_oe_desc { get; private set; }
        public decimal? prt_pris1_od { get; private set; }
        public int? id_prt_pris1_od_desc { get; private set; }
        public decimal? prt_pris1_oe { get; private set; }
        public int? id_prt_pris1_oe_desc { get; private set; }
        public decimal? lng_pris2_od { get; private set; }
        public int? id_lng_pris2_od_desc { get; private set; }
        public decimal? lng_pris2_oe { get; private set; }
        public int? id_lng_pris2_oe_desc { get; private set; }
        public decimal? prt_pris2_od { get; private set; }
        public int? id_prt_pris2_od_desc { get; private set; }
        public decimal? prt_pris2_oe { get; private set; }
        public int? id_prt_pris2_oe_desc { get; private set; }
        public decimal? dist_vertice { get; private set; }
        public decimal? inc_planto { get; private set; }
        public string? ativ_princ { get; private set; }
        public string? otico_area_privilegiada { get; private set; }
        public decimal? dist_visao { get; private set; }
        public string? personalizada { get; private set; }
        public decimal? pris_od { get; private set; }
        public decimal? pris_oe { get; private set; }
        public decimal? eixo_od { get; private set; }
        public decimal? eixo_oe { get; private set; }
        public decimal? borda_od { get; private set; }
        public decimal? borda_oe { get; private set; }
        public decimal? centro_od { get; private set; }
        public decimal? centro_oe { get; private set; }
        public decimal? furo_od { get; private set; }
        public decimal? furo_oe { get; private set; }
        public string? format_corte { get; private set; }
        public string? dist_leitura { get; private set; }
        public string? tipo_lente { get; private set; }
        public string? endereco_paciente { get; private set; }
        public string? medico_especialista { get; private set; }
        public string? endereco_medico_especialista { get; private set; }
        public string? refracionista_optico { get; private set; }
        public DateTime? data_receita { get; private set; }
        public int? id_produtos_opticos_tipo_aro { get; private set; }
        public decimal? dp_montagem { get; private set; }
        public int? id_produtos_opticos_formato_aro { get; private set; }
        public string? codigo_gerencial { get; private set; }
        public bool? migracao_dados { get; private set; }
        public string? migracao_observacao { get; private set; }
        public int? portal { get; private set; }
        public long timestamp { get; private set; }

        public LinxOticoReceitas() { }

        public LinxOticoReceitas(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Optics.LinxOticoReceitas dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_otico_receitas = MicrovixConverters.ToInt32(dto.id_otico_receitas);
            cod_cliente = MicrovixConverters.ToNullableInt32(dto.cod_cliente);
            codigoproduto = MicrovixConverters.ToNullableInt64(dto.codigoproduto);
            lng_esferico_od = MicrovixConverters.ToNullableDecimal(dto.lng_esferico_od);
            lng_cilindrico_od = MicrovixConverters.ToNullableDecimal(dto.lng_cilindrico_od);
            lng_eixo_od = MicrovixConverters.ToNullableDecimal(dto.lng_eixo_od);
            lng_dpdpn_od = MicrovixConverters.ToNullableDecimal(dto.lng_dpdpn_od);
            lng_altura_od = MicrovixConverters.ToNullableDecimal(dto.lng_altura_od);
            prt_esferico_od = MicrovixConverters.ToNullableDecimal(dto.prt_esferico_od);
            prt_cilindrico_od = MicrovixConverters.ToNullableDecimal(dto.prt_cilindrico_od);
            prt_eixo_od = MicrovixConverters.ToNullableDecimal(dto.prt_eixo_od);
            prt_dpdpn_od = MicrovixConverters.ToNullableDecimal(dto.prt_dpdpn_od);
            lng_esferico_oe = MicrovixConverters.ToNullableDecimal(dto.lng_esferico_oe);
            lng_cilindrico_oe = MicrovixConverters.ToNullableDecimal(dto.lng_cilindrico_oe);
            lng_eixo_oe = MicrovixConverters.ToNullableDecimal(dto.lng_eixo_oe);
            lng_dpdpn_oe = MicrovixConverters.ToNullableDecimal(dto.lng_dpdpn_oe);
            lng_altura_oe = MicrovixConverters.ToNullableDecimal(dto.lng_altura_oe);
            prt_esferico_oe = MicrovixConverters.ToNullableDecimal(dto.prt_esferico_oe);
            prt_cilindrico_oe = MicrovixConverters.ToNullableDecimal(dto.prt_cilindrico_oe);
            prt_eixo_oe = MicrovixConverters.ToNullableDecimal(dto.prt_eixo_oe);
            prt_dpdpn_oe = MicrovixConverters.ToNullableDecimal(dto.prt_dpdpn_oe);
            lng_adicao_od = MicrovixConverters.ToNullableDecimal(dto.lng_adicao_od);
            lng_adicao_oe = MicrovixConverters.ToNullableDecimal(dto.lng_adicao_oe);
            otico_lado_lente = MicrovixConverters.ToNullableString(dto.otico_lado_lente, 15);
            diametro_od = MicrovixConverters.ToNullableDecimal(dto.diametro_od);
            diametro_oe = MicrovixConverters.ToNullableDecimal(dto.diametro_oe);
            ponte = MicrovixConverters.ToNullableDecimal(dto.ponte);
            altura = MicrovixConverters.ToNullableDecimal(dto.altura);
            aro = MicrovixConverters.ToNullableDecimal(dto.aro);
            diag_maior = MicrovixConverters.ToNullableDecimal(dto.diag_maior);
            tipo_armacao = MicrovixConverters.ToNullableString(dto.tipo_armacao, 100);
            observacao = MicrovixConverters.ToNullableString(dto.observacao, 100);
            paciente = MicrovixConverters.ToNullableString(dto.paciente, 100);
            cmpl_add_od = MicrovixConverters.ToNullableDecimal(dto.cmpl_add_od);
            cmpl_add_oe = MicrovixConverters.ToNullableDecimal(dto.cmpl_add_oe);
            base_od = MicrovixConverters.ToNullableDecimal(dto.base_od);
            base_oe = MicrovixConverters.ToNullableDecimal(dto.base_oe);
            dist_haste = MicrovixConverters.ToNullableDecimal(dto.dist_haste);
            dist_front = MicrovixConverters.ToNullableDecimal(dto.dist_front);
            curvatura = MicrovixConverters.ToNullableDecimal(dto.curvatura);
            lng_pris1_od = MicrovixConverters.ToNullableDecimal(dto.lng_pris1_od);
            id_lng_pris1_od_desc = MicrovixConverters.ToNullableInt32(dto.id_lng_pris1_od_desc);
            lng_pris1_oe = MicrovixConverters.ToNullableDecimal(dto.lng_pris1_oe);
            id_lng_pris1_oe_desc = MicrovixConverters.ToNullableInt32(dto.id_lng_pris1_oe_desc);
            prt_pris1_od = MicrovixConverters.ToNullableDecimal(dto.prt_pris1_od);
            id_prt_pris1_od_desc = MicrovixConverters.ToNullableInt32(dto.id_prt_pris1_od_desc);
            prt_pris1_oe = MicrovixConverters.ToNullableDecimal(dto.prt_pris1_oe);
            id_prt_pris1_oe_desc = MicrovixConverters.ToNullableInt32(dto.id_prt_pris1_oe_desc);
            lng_pris2_od = MicrovixConverters.ToNullableDecimal(dto.lng_pris2_od);
            id_lng_pris2_od_desc = MicrovixConverters.ToNullableInt32(dto.id_lng_pris2_od_desc);
            lng_pris2_oe = MicrovixConverters.ToNullableDecimal(dto.lng_pris2_oe);
            id_lng_pris2_oe_desc = MicrovixConverters.ToNullableInt32(dto.id_lng_pris2_oe_desc);
            prt_pris2_od = MicrovixConverters.ToNullableDecimal(dto.prt_pris2_od);
            id_prt_pris2_od_desc = MicrovixConverters.ToNullableInt32(dto.id_prt_pris2_od_desc);
            prt_pris2_oe = MicrovixConverters.ToNullableDecimal(dto.prt_pris2_oe);
            id_prt_pris2_oe_desc = MicrovixConverters.ToNullableInt32(dto.id_prt_pris2_oe_desc);
            dist_vertice = MicrovixConverters.ToNullableDecimal(dto.dist_vertice);
            inc_planto = MicrovixConverters.ToNullableDecimal(dto.inc_planto);
            ativ_princ = MicrovixConverters.ToNullableString(dto.ativ_princ, 40);
            otico_area_privilegiada = MicrovixConverters.ToNullableString(dto.otico_area_privilegiada, 15);
            dist_visao = MicrovixConverters.ToNullableDecimal(dto.dist_visao);
            personalizada = MicrovixConverters.ToNullableString(dto.personalizada, 100);
            pris_od = MicrovixConverters.ToNullableDecimal(dto.pris_od);
            pris_oe = MicrovixConverters.ToNullableDecimal(dto.pris_oe);
            eixo_od = MicrovixConverters.ToNullableDecimal(dto.eixo_od);
            eixo_oe = MicrovixConverters.ToNullableDecimal(dto.eixo_oe);
            borda_od = MicrovixConverters.ToNullableDecimal(dto.borda_od);
            borda_oe = MicrovixConverters.ToNullableDecimal(dto.borda_oe);
            centro_od = MicrovixConverters.ToNullableDecimal(dto.centro_od);
            centro_oe = MicrovixConverters.ToNullableDecimal(dto.centro_oe);
            furo_od = MicrovixConverters.ToNullableDecimal(dto.furo_od);
            furo_oe = MicrovixConverters.ToNullableDecimal(dto.furo_oe);
            format_corte = MicrovixConverters.ToNullableString(dto.format_corte, 40);
            dist_leitura = MicrovixConverters.ToNullableString(dto.dist_leitura, 40);
            tipo_lente = MicrovixConverters.ToNullableString(dto.tipo_lente, 100);
            endereco_paciente = MicrovixConverters.ToNullableString(dto.endereco_paciente, 100);
            medico_especialista = MicrovixConverters.ToNullableString(dto.medico_especialista, 100);
            endereco_medico_especialista = MicrovixConverters.ToNullableString(dto.endereco_medico_especialista, 100);
            refracionista_optico = MicrovixConverters.ToNullableString(dto.refracionista_optico, 100);
            data_receita = MicrovixConverters.ToNullableDateTime(dto.data_receita);
            id_produtos_opticos_tipo_aro = MicrovixConverters.ToNullableInt32(dto.id_produtos_opticos_tipo_aro);
            dp_montagem = MicrovixConverters.ToNullableDecimal(dto.dp_montagem);
            id_produtos_opticos_formato_aro = MicrovixConverters.ToNullableInt32(dto.id_produtos_opticos_formato_aro);
            codigo_gerencial = MicrovixConverters.ToNullableString(dto.codigo_gerencial, 40);
            migracao_dados = MicrovixConverters.ToNullableBoolean(dto.migracao_dados);
            migracao_observacao = String.IsNullOrEmpty(dto.migracao_observacao) ? null : dto.migracao_observacao;
            portal = MicrovixConverters.ToNullableInt32(dto.portal);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
