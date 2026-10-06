using System.ComponentModel.DataAnnotations;

namespace Domain.LinxMicrovix.Outbound.WebService.CustomValidations
{
    public static class ConvertToInt32Validation
    {
        public static bool IsValid(string text, string _propertyName, List<ValidationResult> validationResults)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            if (!Int32.TryParse(text, out var number))
            {
                validationResults.Add(new ValidationResult($"Property: {_propertyName} | Value: {text}, Error when trying to convert value: {text} to Int32"));
                return false;
            }
            else
                return true;
        }
    }
}
