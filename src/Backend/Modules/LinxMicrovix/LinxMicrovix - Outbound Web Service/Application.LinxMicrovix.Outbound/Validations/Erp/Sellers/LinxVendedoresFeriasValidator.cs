using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Sellers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script031_Create_LinxMicrovix_Sellers_Tables
    /// </summary>
    public class LinxVendedoresFeriasValidator : AbstractValidator<LinxVendedoresFerias>
    {
        public LinxVendedoresFeriasValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.cod_vendedor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_vendedor));

            RuleFor(x => x.data_inicial)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_inicial));

            RuleFor(x => x.data_final)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_final));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
