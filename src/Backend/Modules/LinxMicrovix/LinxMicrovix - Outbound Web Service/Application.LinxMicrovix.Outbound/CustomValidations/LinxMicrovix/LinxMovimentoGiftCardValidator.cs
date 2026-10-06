using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoGiftCardValidator : AbstractValidator<LinxMovimentoGiftCard>
    {
        public LinxMovimentoGiftCardValidator()
        {
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.data_transacao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_transacao));
            RuleFor(x => x.operacao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.operacao));
            RuleFor(x => x.nsu_cliente).MaximumLength(100).WithMessage("O campo 'nsu_cliente' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.nsu_cliente));
            RuleFor(x => x.nsu_host).MaximumLength(100).WithMessage("O campo 'nsu_host' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.nsu_host));
            RuleFor(x => x.valor).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor));
            RuleFor(x => x.qtde_tentativa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.qtde_tentativa));
            RuleFor(x => x.aprovado_barramento).MaximumLength(4).WithMessage("O campo 'aprovado_barramento' deve ter no máximo 4 caracteres.").When(x => !string.IsNullOrEmpty(x.aprovado_barramento));
            RuleFor(x => x.estornada).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.estornada));
            RuleFor(x => x.codigo_loja).MaximumLength(100).WithMessage("O campo 'codigo_loja' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.codigo_loja));
            RuleFor(x => x.identificador_movimento).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador_movimento' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador_movimento));
            RuleFor(x => x.numero_cartao).MaximumLength(100).WithMessage("O campo 'numero_cartao' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.numero_cartao));
            RuleFor(x => x.plano).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.plano));
            RuleFor(x => x.ambiente_producao).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.ambiente_producao));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}