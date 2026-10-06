using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxNaturezaOperacaoValidator : AbstractValidator<LinxNaturezaOperacao>
    {
        public LinxNaturezaOperacaoValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cod_natureza_operacao).MaximumLength(10).WithMessage("O campo 'cod_natureza_operacao' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.cod_natureza_operacao));
            RuleFor(x => x.descricao).MaximumLength(60).WithMessage("O campo 'descricao' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao));
            RuleFor(x => x.soma_relatorios).MaximumLength(1).WithMessage("O campo 'soma_relatorios' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.soma_relatorios));
            RuleFor(x => x.operacao).MaximumLength(2).WithMessage("O campo 'operacao' deve ter no máximo 2 caracteres.").When(x => !string.IsNullOrEmpty(x.operacao));
            RuleFor(x => x.inativa).MaximumLength(1).WithMessage("O campo 'inativa' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.inativa));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.calcula_ipi).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.calcula_ipi));
            RuleFor(x => x.calcula_iss).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.calcula_iss));
            RuleFor(x => x.calcula_irrf).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.calcula_irrf));
            RuleFor(x => x.tipo_preco).MaximumLength(2).WithMessage("O campo 'tipo_preco' deve ter no máximo 2 caracteres.").When(x => !string.IsNullOrEmpty(x.tipo_preco));
            RuleFor(x => x.atualiza_custo).MaximumLength(1).WithMessage("O campo 'atualiza_custo' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.atualiza_custo));
            RuleFor(x => x.transferencia).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transferencia));
            RuleFor(x => x.baixar_estoque).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.baixar_estoque));
            RuleFor(x => x.consumo_proprio).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.consumo_proprio));
            RuleFor(x => x.contabiliza_cmv).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.contabiliza_cmv));
            RuleFor(x => x.despesa).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.despesa));
            RuleFor(x => x.atualiza_custo_medio).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.atualiza_custo_medio));
            RuleFor(x => x.exige_nf_origem).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.exige_nf_origem));
            RuleFor(x => x.integra_contabilidade).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.integra_contabilidade));
            RuleFor(x => x.id_obs).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_obs));
            RuleFor(x => x.venda_futura).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.venda_futura));
            RuleFor(x => x.base_icms_considera_ipi).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.base_icms_considera_ipi));
            RuleFor(x => x.permite_escolha_historico).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.permite_escolha_historico));
            RuleFor(x => x.import_produtos).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.import_produtos));
            RuleFor(x => x.deposito_reserva_venda).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.deposito_reserva_venda));
            RuleFor(x => x.exibe_nfe).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.exibe_nfe));
            RuleFor(x => x.faturamento_antecipado).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.faturamento_antecipado));
            RuleFor(x => x.exibir_informacoes_imposto).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.exibir_informacoes_imposto));
            RuleFor(x => x.gera_garantia_nacional).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.gera_garantia_nacional));
            RuleFor(x => x.transferencia_deposito).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.transferencia_deposito));
            RuleFor(x => x.venda_diferencial_aliquota).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.venda_diferencial_aliquota));
            RuleFor(x => x.insere_obs_pis_cofins).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.insere_obs_pis_cofins));
            RuleFor(x => x.diferencial_ativo_consumo).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.diferencial_ativo_consumo));
            RuleFor(x => x.recusa_de).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.recusa_de));
            RuleFor(x => x.codigo_ws).MaximumLength(50).WithMessage("O campo 'codigo_ws' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_ws));
        }
    }
}
