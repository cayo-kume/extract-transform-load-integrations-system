using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoPrincipalValidator : AbstractValidator<LinxMovimentoPrincipal>
    {
        public LinxMovimentoPrincipalValidator()
        {
            RuleFor(x => x.id_movimento_principal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_movimento_principal));
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.codigoproduto_manutencao).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.codigoproduto_manutencao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.id_pergunta_venda).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_pergunta_venda));
            RuleFor(x => x.id_resposta_venda).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_resposta_venda));
            RuleFor(x => x.total_fidelidade_cashback).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.total_fidelidade_cashback));
            RuleFor(x => x.plano_fidelidade_cashback).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.plano_fidelidade_cashback));
            RuleFor(x => x.remessa_pedido_compra).MaximumLength(30).WithMessage("O campo 'remessa_pedido_compra' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.remessa_pedido_compra));
            RuleFor(x => x.id_motivo_desconto).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_motivo_desconto));
            RuleFor(x => x.valor_nota).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor_nota));
        }
    }
}