using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Financial;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Financial
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script061_Create_LinxMicrovix_Financial_Tables
    /// </summary>
    public class LinxExtratoBancarioLancamentosValidator : AbstractValidator<LinxExtratoBancarioLancamentos>
    {
        public LinxExtratoBancarioLancamentosValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_extrato_bancario_lancamentos)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_extrato_bancario_lancamentos' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.codigo_banco)
                .MaximumLength(4)
                .WithMessage("O campo 'codigo_banco' deve ter no máximo 4 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_banco));

            RuleFor(x => x.numero_conta)
                .MaximumLength(20)
                .WithMessage("O campo 'numero_conta' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_conta));

            RuleFor(x => x.id_conta)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id_conta));

            RuleFor(x => x.valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.descricao)
                .MaximumLength(100)
                .WithMessage("O campo 'descricao' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.data_lancamento)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_lancamento));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
