using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Customers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script028_Create_LinxMicrovix_Customers_Tables
    /// </summary>
    public class LinxClientesFornecClassesValidator : AbstractValidator<LinxClientesFornecClasses>
    {
        public LinxClientesFornecClassesValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cod_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente));

            RuleFor(x => x.cod_classe)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_classe));

            RuleFor(x => x.nome_classe)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_classe' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_classe));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
