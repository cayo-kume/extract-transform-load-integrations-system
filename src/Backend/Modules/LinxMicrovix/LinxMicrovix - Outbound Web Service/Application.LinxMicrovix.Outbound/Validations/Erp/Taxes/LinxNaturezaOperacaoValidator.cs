using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxNaturezaOperacaoValidator : AbstractValidator<LinxNaturezaOperacao>
    {
        public LinxNaturezaOperacaoValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cod_natureza_operacao)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'cod_natureza_operacao' é obrigatório.")
                .MaximumLength(10)
                .WithMessage("O campo 'cod_natureza_operacao' deve ter no máximo 10 caracteres.");

            RuleFor(x => x.descricao)
                .MaximumLength(60)
                .WithMessage("O campo 'descricao' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.soma_relatorios)
                .MaximumLength(1)
                .WithMessage("O campo 'soma_relatorios' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.soma_relatorios));

            RuleFor(x => x.operacao)
                .MaximumLength(2)
                .WithMessage("O campo 'operacao' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operacao));

            RuleFor(x => x.inativa)
                .MaximumLength(1)
                .WithMessage("O campo 'inativa' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.inativa));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.calcula_ipi)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.calcula_ipi));

            RuleFor(x => x.calcula_iss)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.calcula_iss));

            RuleFor(x => x.calcula_irrf)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.calcula_irrf));

            RuleFor(x => x.tipo_preco)
                .MaximumLength(2)
                .WithMessage("O campo 'tipo_preco' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_preco));

            RuleFor(x => x.atualiza_custo)
                .MaximumLength(1)
                .WithMessage("O campo 'atualiza_custo' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.atualiza_custo));

            RuleFor(x => x.transferencia)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.transferencia));

            RuleFor(x => x.baixar_estoque)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.baixar_estoque));

            RuleFor(x => x.consumo_proprio)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.consumo_proprio));

            RuleFor(x => x.contabiliza_cmv)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.contabiliza_cmv));

            RuleFor(x => x.despesa)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.despesa));

            RuleFor(x => x.atualiza_custo_medio)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.atualiza_custo_medio));

            RuleFor(x => x.exige_nf_origem)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.exige_nf_origem));

            RuleFor(x => x.integra_contabilidade)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.integra_contabilidade));

            RuleFor(x => x.id_obs)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_obs));

            RuleFor(x => x.venda_futura)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.venda_futura));

            RuleFor(x => x.base_icms_considera_ipi)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.base_icms_considera_ipi));

            RuleFor(x => x.permite_escolha_historico)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.permite_escolha_historico));

            RuleFor(x => x.import_produtos)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.import_produtos));

            RuleFor(x => x.deposito_reserva_venda)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.deposito_reserva_venda));

            RuleFor(x => x.exibe_nfe)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.exibe_nfe));

            RuleFor(x => x.faturamento_antecipado)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.faturamento_antecipado));

            RuleFor(x => x.exibir_informacoes_imposto)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.exibir_informacoes_imposto));

            RuleFor(x => x.gera_garantia_nacional)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.gera_garantia_nacional));

            RuleFor(x => x.transferencia_deposito)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.transferencia_deposito));

            RuleFor(x => x.venda_diferencial_aliquota)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.venda_diferencial_aliquota));

            RuleFor(x => x.insere_obs_pis_cofins)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.insere_obs_pis_cofins));

            RuleFor(x => x.diferencial_ativo_consumo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.diferencial_ativo_consumo));

            RuleFor(x => x.recusa_de)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.recusa_de));

            RuleFor(x => x.codigo_ws)
                .MaximumLength(50)
                .WithMessage("O campo 'codigo_ws' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_ws));
        }
    }
}
