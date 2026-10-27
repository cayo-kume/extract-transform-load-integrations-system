using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Loyalty;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Loyalty
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script037_Create_LinxMicrovix_Loyalty_Tables
    /// </summary>
    public class LinxMovimentoReshopValidator : AbstractValidator<LinxMovimentoReshop>
    {
        public LinxMovimentoReshopValidator()
        {
            RuleFor(x => x.id_movimento_campanha_reshop)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_movimento_campanha_reshop' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_campanha)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_campanha));

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.nome)
                .MaximumLength(100)
                .WithMessage("O campo 'nome' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome));

            RuleFor(x => x.descricao)
                .MaximumLength(200)
                .WithMessage("O campo 'descricao' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.aplicar_desconto_venda)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.aplicar_desconto_venda));

            RuleFor(x => x.valor_desconto_subtotal)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_desconto_subtotal));

            RuleFor(x => x.valor_desconto_completo)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_desconto_completo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
