using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Payments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script034_Create_LinxMicrovix_Payments_Tables
    /// </summary>
    public class LinxMovimentoPlanosValidator : AbstractValidator<LinxMovimentoPlanos>
    {
        public LinxMovimentoPlanosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.identificador)
                .MustBeValidGuid()
                .When(x => !string.IsNullOrEmpty(x.identificador));

            RuleFor(x => x.plano)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano));

            RuleFor(x => x.desc_plano)
                .MaximumLength(100)
                .WithMessage("O campo 'desc_plano' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.desc_plano));

            RuleFor(x => x.total)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.total));

            RuleFor(x => x.qtde_parcelas)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.qtde_parcelas));

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

            RuleFor(x => x.tipo_transacao)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_transacao' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_transacao));

            RuleFor(x => x.taxa_financeira)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.taxa_financeira));

            RuleFor(x => x.ordem_cartao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ordem_cartao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.gerado_config_receb_vendas)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.gerado_config_receb_vendas));
        }
    }
}
