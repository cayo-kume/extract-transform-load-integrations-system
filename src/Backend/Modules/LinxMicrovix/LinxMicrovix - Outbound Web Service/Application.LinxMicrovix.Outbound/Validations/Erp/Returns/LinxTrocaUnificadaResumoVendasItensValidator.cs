using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Returns
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script046_Create_LinxMicrovix_Returns_Tables
    /// </summary>
    public class LinxTrocaUnificadaResumoVendasItensValidator : AbstractValidator<LinxTrocaUnificadaResumoVendasItens>
    {
        public LinxTrocaUnificadaResumoVendasItensValidator()
        {
            RuleFor(x => x.id_troca_unificada_resumo_vendas_itens)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_troca_unificada_resumo_vendas_itens' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.id_troca_unificada_resumo_vendas)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id_troca_unificada_resumo_vendas));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.transacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao));

            RuleFor(x => x.serial)
                .MaximumLength(50)
                .WithMessage("O campo 'serial' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serial));

            RuleFor(x => x.valor_liquido)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_liquido));

            RuleFor(x => x.data_validade)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_validade));

            RuleFor(x => x.venda_referenciada)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.venda_referenciada));

            RuleFor(x => x.token_utilizado)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.token_utilizado));

            RuleFor(x => x.quantidade)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.quantidade));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
