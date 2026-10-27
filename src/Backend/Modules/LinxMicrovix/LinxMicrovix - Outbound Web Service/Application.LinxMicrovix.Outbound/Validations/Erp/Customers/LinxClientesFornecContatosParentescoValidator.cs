using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Customers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script028_Create_LinxMicrovix_Customers_Tables
    /// </summary>
    public class LinxClientesFornecContatosParentescoValidator : AbstractValidator<LinxClientesFornecContatosParentesco>
    {
        public LinxClientesFornecContatosParentescoValidator()
        {
            RuleFor(x => x.id_parentesco)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_parentesco' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao_parentesco)
                .MaximumLength(50)
                .WithMessage("O campo 'descricao_parentesco' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_parentesco));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
