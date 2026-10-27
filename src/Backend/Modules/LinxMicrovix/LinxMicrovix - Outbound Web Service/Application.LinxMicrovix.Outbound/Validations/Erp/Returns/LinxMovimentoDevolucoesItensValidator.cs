using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Returns
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script046_Create_LinxMicrovix_Returns_Tables
    /// </summary>
    public class LinxMovimentoDevolucoesItensValidator : AbstractValidator<LinxMovimentoDevolucoesItens>
    {
        public LinxMovimentoDevolucoesItensValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.identificador_venda)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_venda));

            RuleFor(x => x.identificador_devolucao)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_devolucao));

            RuleFor(x => x.codigoproduto)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto));

            RuleFor(x => x.id_movimento_devolucoes_itens)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_movimento_devolucoes_itens' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.transacao_origem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao_origem));

            RuleFor(x => x.transacao_devolucao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.transacao_devolucao));

            RuleFor(x => x.qtde_devolvida)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.qtde_devolvida));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
