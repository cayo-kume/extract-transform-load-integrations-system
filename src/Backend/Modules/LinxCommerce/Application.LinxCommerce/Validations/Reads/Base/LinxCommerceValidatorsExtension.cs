using Domain.LinxCommerce.Extensions;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Reads.Base
{
    /// <summary>
    /// regras do módulo que o Shared não tem: a data com fuso (ou no formato do WCF) e o decimal com ponto, que o
    /// MustBeValidDecimal lê na cultura da máquina (pt-BR)
    /// </summary>
    public static class LinxCommerceValidatorsExtension
    {
        public static IRuleBuilderOptions<T, string?> MustBeValidDateTimeOffset<T>(this IRuleBuilder<T, string?> ruleBuilder) =>
            ruleBuilder.Must(value => LinxCommerceConverters.ToNullableDateTimeOffset(value) is not null);

        public static IRuleBuilderOptions<T, string?> MustBeValidInvariantDecimal<T>(this IRuleBuilder<T, string?> ruleBuilder) =>
            ruleBuilder.Must(value => NullableConverters.ToNullableDecimal(value) is not null);
    }
}
