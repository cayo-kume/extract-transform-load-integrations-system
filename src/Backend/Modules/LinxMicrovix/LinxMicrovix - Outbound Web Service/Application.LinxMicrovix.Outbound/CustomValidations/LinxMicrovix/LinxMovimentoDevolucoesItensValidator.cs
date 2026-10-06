using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoDevolucoesItensValidator : AbstractValidator<LinxMovimentoDevolucoesItens>
    {
        public LinxMovimentoDevolucoesItensValidator()
        {
            RuleFor(x => x.id_movimento_devolucoes_itens).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_movimento_devolucoes_itens));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.identificador_venda).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_venda' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_venda));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.identificador_devolucao).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_devolucao' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_devolucao));
            RuleFor(x => x.codigoproduto).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto));
            RuleFor(x => x.transacao_origem).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao_origem));
            RuleFor(x => x.transacao_devolucao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.transacao_devolucao));
            RuleFor(x => x.qtde_devolvida).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.qtde_devolvida));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
