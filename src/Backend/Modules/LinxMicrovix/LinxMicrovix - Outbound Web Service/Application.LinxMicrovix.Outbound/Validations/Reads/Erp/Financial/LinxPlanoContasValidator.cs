using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxPlanoContasValidator : AbstractValidator<LinxPlanoContas>
    {
        public LinxPlanoContasValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.CNPJ)
                .MaximumLength(14)
                .WithMessage("O campo 'CNPJ' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.CNPJ));

            RuleFor(x => x.id_conta)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_conta));

            RuleFor(x => x.nome_conta)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_conta' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_conta));

            RuleFor(x => x.sintetica)
                .MaximumLength(1)
                .WithMessage("O campo 'sintetica' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sintetica));

            RuleFor(x => x.indice)
                .MaximumLength(150)
                .WithMessage("O campo 'indice' deve ter no máximo 150 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.indice));

            RuleFor(x => x.ativa)
                .MaximumLength(1)
                .WithMessage("O campo 'ativa' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.ativa));

            RuleFor(x => x.fluxo_caixa)
                .MaximumLength(1)
                .WithMessage("O campo 'fluxo_caixa' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fluxo_caixa));

            RuleFor(x => x.CONTA_CONTABIL)
                .MaximumLength(20)
                .WithMessage("O campo 'CONTA_CONTABIL' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.CONTA_CONTABIL));

            RuleFor(x => x.id_natureza_conta)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_natureza_conta));

            RuleFor(x => x.conta_bancaria)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.conta_bancaria));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
