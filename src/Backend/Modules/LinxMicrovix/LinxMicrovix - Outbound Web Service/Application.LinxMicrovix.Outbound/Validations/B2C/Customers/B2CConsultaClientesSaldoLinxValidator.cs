using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.Customers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script070_Create_LinxMicrovix_B2C_Customers_Tables
    /// </summary>
    public class B2CConsultaClientesSaldoLinxValidator : AbstractValidator<B2CConsultaClientesSaldoLinx>
    {
        public B2CConsultaClientesSaldoLinxValidator()
        {
            RuleFor(x => x.cod_cliente_erp)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_erp));

            RuleFor(x => x.cod_cliente_b2c)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_b2c));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.valor)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
