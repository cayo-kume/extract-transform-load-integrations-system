using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Payments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script034_Create_LinxMicrovix_Payments_Tables
    /// </summary>
    public class LinxMovimentoGiftCardValidator : AbstractValidator<LinxMovimentoGiftCard>
    {
        public LinxMovimentoGiftCardValidator()
        {
            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.data_transacao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_transacao));

            RuleFor(x => x.operacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.operacao));

            RuleFor(x => x.nsu_cliente)
                .MaximumLength(100)
                .WithMessage("O campo 'nsu_cliente' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nsu_cliente));

            RuleFor(x => x.nsu_host)
                .MaximumLength(100)
                .WithMessage("O campo 'nsu_host' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nsu_host));

            RuleFor(x => x.valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.qtde_tentativa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtde_tentativa));

            RuleFor(x => x.aprovado_barramento)
                .MaximumLength(4)
                .WithMessage("O campo 'aprovado_barramento' deve ter no máximo 4 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.aprovado_barramento));

            RuleFor(x => x.estornada)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.estornada));

            RuleFor(x => x.codigo_loja)
                .MaximumLength(100)
                .WithMessage("O campo 'codigo_loja' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_loja));

            RuleFor(x => x.identificador_movimento)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador_movimento));

            RuleFor(x => x.numero_cartao)
                .MaximumLength(100)
                .WithMessage("O campo 'numero_cartao' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_cartao));

            RuleFor(x => x.plano)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano));

            RuleFor(x => x.ambiente_producao)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ambiente_producao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
