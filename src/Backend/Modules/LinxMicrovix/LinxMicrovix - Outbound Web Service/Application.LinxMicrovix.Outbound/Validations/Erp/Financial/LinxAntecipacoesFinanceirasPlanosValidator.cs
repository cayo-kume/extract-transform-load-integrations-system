using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxAntecipacoesFinanceirasPlanosValidator : AbstractValidator<LinxAntecipacoesFinanceirasPlanos>
    {
        public LinxAntecipacoesFinanceirasPlanosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.id_antecipacoes_financeiras)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_antecipacoes_financeiras));

            RuleFor(x => x.numero_antecipacao)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.numero_antecipacao));

            RuleFor(x => x.forma_pgto)
                .MaximumLength(50)
                .WithMessage("O campo 'forma_pgto' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.forma_pgto));

            RuleFor(x => x.plano)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.plano));

            RuleFor(x => x.nome_plano)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_plano' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_plano));

            RuleFor(x => x.valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.cancelado)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.cancelado));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.data_criacao_antecipacao_plano)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_criacao_antecipacao_plano));
        }
    }
}
