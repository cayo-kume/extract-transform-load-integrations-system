using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.PosSales;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.PosSales
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script043_Create_LinxMicrovix_Pos_Sales_Tables
    /// </summary>
    public class LinxVendasPosProdutosTmpValidator : AbstractValidator<LinxVendasPosProdutosTmp>
    {
        public LinxVendasPosProdutosTmpValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_vendas_pos_produtos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_vendas_pos_produtos' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.numero_ficha)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.numero_ficha));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.preco_unitario)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_unitario));

            RuleFor(x => x.preco_tabela)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_tabela));

            RuleFor(x => x.quantidade)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantidade));

            RuleFor(x => x.vendedor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.vendedor));

            RuleFor(x => x.desconto_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto_item));

            RuleFor(x => x.id_tabela_preco)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_tabela_preco));

            RuleFor(x => x.altura)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.altura));

            RuleFor(x => x.comprimento)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.comprimento));

            RuleFor(x => x.largura)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.largura));

            RuleFor(x => x.cfop_item)
                .MaximumLength(10)
                .WithMessage("O campo 'cfop_item' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cfop_item));

            RuleFor(x => x.excluido)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluido));

            RuleFor(x => x.aliq_icms)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliq_icms));

            RuleFor(x => x.cst_fiscal)
                .MaximumLength(4)
                .WithMessage("O campo 'cst_fiscal' deve ter no máximo 4 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cst_fiscal));

            RuleFor(x => x.indice_ordenacao_espelho)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.indice_ordenacao_espelho));

            RuleFor(x => x.id_vendas_pos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_vendas_pos));

            RuleFor(x => x.id_cst_icms_fiscal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cst_icms_fiscal));

            RuleFor(x => x.id_cst_ipi_fiscal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cst_ipi_fiscal));

            RuleFor(x => x.id_cst_pis_fiscal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cst_pis_fiscal));

            RuleFor(x => x.id_cst_cofins_fiscal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cst_cofins_fiscal));

            RuleFor(x => x.aliq_iss)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliq_iss));

            RuleFor(x => x.id_cfop_fiscal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_cfop_fiscal));

            RuleFor(x => x.aliq_pis)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliq_pis));

            RuleFor(x => x.aliq_cofins)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.aliq_cofins));

            RuleFor(x => x.desconto_item_valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto_item_valor));

            RuleFor(x => x.lote)
                .MaximumLength(60)
                .WithMessage("O campo 'lote' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.lote));

            RuleFor(x => x.deposito)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.deposito));

            RuleFor(x => x.previsao_entrega)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.previsao_entrega));

            RuleFor(x => x.transacao_movimento_reserva)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao_movimento_reserva));

            RuleFor(x => x.data_exclusao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_exclusao));

            RuleFor(x => x.desconto_total_item)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.desconto_total_item));

            RuleFor(x => x.id_motivo_desconto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_motivo_desconto));

            RuleFor(x => x.motivo_desconto_observacao)
                .MaximumLength(500)
                .WithMessage("O campo 'motivo_desconto_observacao' deve ter no máximo 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.motivo_desconto_observacao));

            RuleFor(x => x.crm_cliente_indicador)
                .MaximumLength(18)
                .WithMessage("O campo 'crm_cliente_indicador' deve ter no máximo 18 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.crm_cliente_indicador));

            RuleFor(x => x.cod_cliente_indicador)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_indicador));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
