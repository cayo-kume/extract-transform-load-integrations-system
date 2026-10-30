using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Movements;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Movements
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script040_Create_LinxMicrovix_Movements_Tables
    /// </summary>
    public class LinxMovimentoPrincipalValidator : AbstractValidator<LinxMovimentoPrincipal>
    {
        public LinxMovimentoPrincipalValidator()
        {
            RuleFor(x => x.id_movimento_principal)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_movimento_principal' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.codigoproduto_manutencao)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.codigoproduto_manutencao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.id_pergunta_venda)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_pergunta_venda));

            RuleFor(x => x.id_resposta_venda)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_resposta_venda));

            RuleFor(x => x.total_fidelidade_cashback)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.total_fidelidade_cashback));

            RuleFor(x => x.plano_fidelidade_cashback)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano_fidelidade_cashback));

            RuleFor(x => x.remessa_pedido_compra)
                .MaximumLength(30)
                .WithMessage("O campo 'remessa_pedido_compra' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.remessa_pedido_compra));

            RuleFor(x => x.id_motivo_desconto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_motivo_desconto));

            RuleFor(x => x.valor_nota)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_nota));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.estornada)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.estornada));

            RuleFor(x => x.motivo_desconto_observacao)
                .MaximumLength(500)
                .WithMessage("O campo 'motivo_desconto_observacao' deve ter no máximo 500 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.motivo_desconto_observacao));

            RuleFor(x => x.nota_aguardou_sefaz)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.nota_aguardou_sefaz));
        }
    }
}
