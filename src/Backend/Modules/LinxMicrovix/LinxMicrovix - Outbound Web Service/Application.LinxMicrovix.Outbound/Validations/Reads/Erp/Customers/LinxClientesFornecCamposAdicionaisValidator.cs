using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Customers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Customers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script028_Create_LinxMicrovix_Customers_Tables
    /// </summary>
    public class LinxClientesFornecCamposAdicionaisValidator : AbstractValidator<LinxClientesFornecCamposAdicionais>
    {
        public LinxClientesFornecCamposAdicionaisValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cod_cliente)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente));

            RuleFor(x => x.campo)
                .MaximumLength(1000)
                .WithMessage("O campo 'campo' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.campo));

            RuleFor(x => x.valor)
                .MaximumLength(100)
                .WithMessage("O campo 'valor' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
