using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Sellers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script031_Create_LinxMicrovix_Sellers_Tables
    /// </summary>
    public class LinxFuncionariosFuncoesValidator : AbstractValidator<LinxFuncionariosFuncoes>
    {
        public LinxFuncionariosFuncoesValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_funcionarios_funcao)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_funcionarios_funcao' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(100)
                .WithMessage("O campo 'descricao' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
