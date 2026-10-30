using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Prices;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Prices
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script025_Create_LinxMicrovix_Prices_Tables
    /// </summary>
    public class LinxAcoesPromocionaisValidator : AbstractValidator<LinxAcoesPromocionais>
    {
        public LinxAcoesPromocionaisValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.id_acoes_promocionais)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_acoes_promocionais' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(100)
                .WithMessage("O campo 'descricao' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.vigencia_inicio)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.vigencia_inicio));

            RuleFor(x => x.vigencia_fim)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.vigencia_fim));

            RuleFor(x => x.ativa)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativa));

            RuleFor(x => x.excluida)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluida));

            RuleFor(x => x.integrada)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.integrada));

            RuleFor(x => x.qtde_integrada)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtde_integrada));

            RuleFor(x => x.valor_pago_franqueadora)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_pago_franqueadora));

            RuleFor(x => x.id_acoes_promocionais_origem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_acoes_promocionais_origem));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
