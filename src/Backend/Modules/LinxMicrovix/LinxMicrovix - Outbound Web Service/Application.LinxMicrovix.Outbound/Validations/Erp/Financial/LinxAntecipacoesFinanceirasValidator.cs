using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxAntecipacoesFinanceirasValidator : AbstractValidator<LinxAntecipacoesFinanceiras>
    {
        public LinxAntecipacoesFinanceirasValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.cod_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente));

            RuleFor(x => x.numero_antecipacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_antecipacao));

            RuleFor(x => x.data_antecipacao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_antecipacao));

            RuleFor(x => x.previsao_entrega)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.previsao_entrega));

            RuleFor(x => x.dav_pv)
                .MaximumLength(3)
                .WithMessage("O campo 'dav_pv' deve ter no máximo 3 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.dav_pv));

            RuleFor(x => x.numero_origem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_origem));

            RuleFor(x => x.dav_remessa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.dav_remessa));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.quantidade)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantidade));

            RuleFor(x => x.preco_unitario_produto)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.preco_unitario_produto));

            RuleFor(x => x.valor_pago_antecipacao)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_pago_antecipacao));

            RuleFor(x => x.entregue)
                .MaximumLength(1)
                .WithMessage("O campo 'entregue' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.entregue));

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.cancelado)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.cancelado));

            RuleFor(x => x.id_antecipacoes_financeiras)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_antecipacoes_financeiras));

            RuleFor(x => x.id_antecipacoes_financeiras_detalhes)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_antecipacoes_financeiras_detalhes));

            RuleFor(x => x.id_vendas_pos_produtos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_vendas_pos_produtos));

            RuleFor(x => x.valor_pagar)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_pagar));

            RuleFor(x => x.identificador_faturamento_antecipado)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_faturamento_antecipado));
        }
    }
}
