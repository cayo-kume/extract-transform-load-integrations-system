using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxFaturasValidator : AbstractValidator<LinxFaturas>
    {
        public LinxFaturasValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.codigo_fatura).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigo_fatura));
            RuleFor(x => x.data_emissao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_emissao));
            RuleFor(x => x.cod_cliente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_cliente));
            RuleFor(x => x.nome_cliente).MaximumLength(60).WithMessage("O campo 'nome_cliente' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_cliente));
            RuleFor(x => x.data_vencimento).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_vencimento));
            RuleFor(x => x.data_baixa).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_baixa));
            RuleFor(x => x.valor_fatura).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_fatura));
            RuleFor(x => x.valor_pago).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_pago));
            RuleFor(x => x.valor_desconto).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_desconto));
            RuleFor(x => x.valor_juros).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_juros));
            RuleFor(x => x.documento).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.documento));
            RuleFor(x => x.serie).MaximumLength(10).WithMessage("O campo 'serie' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.serie));
            RuleFor(x => x.ecf).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.ecf));
            RuleFor(x => x.qtde_parcelas).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtde_parcelas));
            RuleFor(x => x.ordem_parcela).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.ordem_parcela));
            RuleFor(x => x.receber_pagar).MaximumLength(1).WithMessage("O campo 'receber_pagar' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.receber_pagar));
            RuleFor(x => x.vendedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.vendedor));
            RuleFor(x => x.excluido).MaximumLength(1).WithMessage("O campo 'excluido' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.excluido));
            RuleFor(x => x.cancelado).MaximumLength(1).WithMessage("O campo 'cancelado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.cancelado));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.nsu).MaximumLength(50).WithMessage("O campo 'nsu' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nsu));
            RuleFor(x => x.cod_autorizacao).MaximumLength(50).WithMessage("O campo 'cod_autorizacao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.cod_autorizacao));
            RuleFor(x => x.documento_sem_tef).MaximumLength(350).WithMessage("O campo 'documento_sem_tef' deve ter no máximo 350 caracteres.").When(x => !string.IsNullOrEmpty(x.documento_sem_tef));
            RuleFor(x => x.autorizacao_sem_tef).MaximumLength(30).WithMessage("O campo 'autorizacao_sem_tef' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.autorizacao_sem_tef));
            RuleFor(x => x.plano).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.plano));
            RuleFor(x => x.conta_credito).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.conta_credito));
            RuleFor(x => x.conta_debito).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.conta_debito));
            RuleFor(x => x.conta_fluxo).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.conta_fluxo));
            RuleFor(x => x.cod_historico).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.cod_historico));
            RuleFor(x => x.forma_pgto).MaximumLength(50).WithMessage("O campo 'forma_pgto' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.forma_pgto));
            RuleFor(x => x.ordem_cartao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.ordem_cartao));
            RuleFor(x => x.banco_codigo).MaximumLength(10).WithMessage("O campo 'banco_codigo' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.banco_codigo));
            RuleFor(x => x.banco_agencia).MaximumLength(30).WithMessage("O campo 'banco_agencia' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.banco_agencia));
            RuleFor(x => x.banco_conta).MaximumLength(30).WithMessage("O campo 'banco_conta' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.banco_conta));
            RuleFor(x => x.banco_autorizacao_garantidora).MaximumLength(30).WithMessage("O campo 'banco_autorizacao_garantidora' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.banco_autorizacao_garantidora));
            RuleFor(x => x.numero_bilhete_seguro).MaximumLength(30).WithMessage("O campo 'numero_bilhete_seguro' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.numero_bilhete_seguro));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.id_categorias_financeiras).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_categorias_financeiras));
            RuleFor(x => x.taxa_financeira).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.taxa_financeira));
            RuleFor(x => x.valor_abatimento).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_abatimento));
            RuleFor(x => x.valor_multa).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_multa));
            RuleFor(x => x.centrocusto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.centrocusto));
            RuleFor(x => x.perc_taxa_adquirente).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.perc_taxa_adquirente));
            RuleFor(x => x.fatura_origem_importacao_erp).MaximumLength(50).WithMessage("O campo 'fatura_origem_importacao_erp' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.fatura_origem_importacao_erp));
        }
    }
}
