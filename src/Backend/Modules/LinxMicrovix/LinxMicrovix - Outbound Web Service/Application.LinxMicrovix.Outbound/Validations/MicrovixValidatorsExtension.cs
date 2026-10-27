using Domain.LinxMicrovix.Outbound.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations
{
    /// <summary>
    /// regras do módulo que o FluentValidatorsExtension do Shared não cobre: BIT do Microvix vem como True/False ou 0/1 (v274 §4.3)
    /// </summary>
    public static class MicrovixValidatorsExtension
    {
        public static IRuleBuilderOptions<T, string?> MustBeValidMicrovixBoolean<T>(this IRuleBuilder<T, string?> ruleBuilder)
        {
            return ruleBuilder
                .Must(value => MicrovixConverters.ToNullableBoolean(value) is not null)
                .WithMessage($"O valor [0] deve ser um booleano válido (True/False ou 0/1).");
        }
    }
}
