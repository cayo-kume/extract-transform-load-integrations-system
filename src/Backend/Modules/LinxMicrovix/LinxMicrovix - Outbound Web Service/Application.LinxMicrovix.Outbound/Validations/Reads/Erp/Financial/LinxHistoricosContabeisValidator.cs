using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxHistoricosContabeisValidator : AbstractValidator<LinxHistoricosContabeis>
    {
        public LinxHistoricosContabeisValidator()
        {
            RuleFor(x => x.id_historico)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_historico' é obrigatório.")
                .MustBeValidInt64();

            RuleFor(x => x.nome_historico)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_historico' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_historico));

            RuleFor(x => x.conta_credito)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.conta_credito));

            RuleFor(x => x.conta_debito)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.conta_debito));

            RuleFor(x => x.tipo_rateio)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_rateio' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo_rateio));

            RuleFor(x => x.inativo)
                .MaximumLength(1)
                .WithMessage("O campo 'inativo' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.inativo));

            RuleFor(x => x.operacao)
                .MaximumLength(1)
                .WithMessage("O campo 'operacao' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.operacao));

            RuleFor(x => x.historico_contabil)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.historico_contabil));

            RuleFor(x => x.somente_gerencial)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.somente_gerencial));

            RuleFor(x => x.obrigatorio_insercao_anexo_sangria)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.obrigatorio_insercao_anexo_sangria));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
