using System.ComponentModel.DataAnnotations;

namespace Domain.LinxMicrovix.Outbound.WebService.CustomValidations
{
    public static class ConvertToInt64Validation
    {
        public static bool IsValid(string text, string _propertyName, List<ValidationResult> validationResults)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            if (!Int64.TryParse(text, out var boolean))
            {
                validationResults.Add(new ValidationResult($"Property: {_propertyName} | Value: {text}, Error when trying to convert value: {text} to Int64"));
                return false;
            }
            else
                return true;
        }
    }
}
