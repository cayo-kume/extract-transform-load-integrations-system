using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Payments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script034_Create_LinxMicrovix_Payments_Tables
    /// </summary>
    public class LinxPlanosValidator : AbstractValidator<LinxPlanos>
    {
        public LinxPlanosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.plano)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'plano' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.desc_plano)
                .MaximumLength(100)
                .WithMessage("O campo 'desc_plano' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_plano));

            RuleFor(x => x.qtde_parcelas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtde_parcelas));

            RuleFor(x => x.prazo_entre_parcelas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.prazo_entre_parcelas));

            RuleFor(x => x.tipo_plano)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_plano' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_plano));

            RuleFor(x => x.indice_plano)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.indice_plano));

            RuleFor(x => x.cod_forma_pgto)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_forma_pgto));

            RuleFor(x => x.forma_pgto)
                .MaximumLength(50)
                .WithMessage("O campo 'forma_pgto' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.forma_pgto));

            RuleFor(x => x.conta_central)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.conta_central));

            RuleFor(x => x.tipo_transacao)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_transacao' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_transacao));

            RuleFor(x => x.taxa_financeira)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.taxa_financeira));

            RuleFor(x => x.dt_upd)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.dt_upd));

            RuleFor(x => x.desativado)
                .MaximumLength(1)
                .WithMessage("O campo 'desativado' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desativado));

            RuleFor(x => x.usa_tef)
                .MaximumLength(1)
                .WithMessage("O campo 'usa_tef' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.usa_tef));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.codigo_ws)
                .MaximumLength(50)
                .WithMessage("O campo 'codigo_ws' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_ws));

            RuleFor(x => x.valor_minimo_parcela)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_minimo_parcela));

            RuleFor(x => x.gerado_config_receb_vendas)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.gerado_config_receb_vendas));
        }
    }
}
