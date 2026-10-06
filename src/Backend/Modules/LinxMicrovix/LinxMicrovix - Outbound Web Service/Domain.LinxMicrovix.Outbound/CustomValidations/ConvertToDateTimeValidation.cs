using System.ComponentModel.DataAnnotations;

namespace Domain.LinxMicrovix.Outbound.WebService.CustomValidations
{
    public static class ConvertToDateTimeValidation
    {
        public static bool IsValid(string text, string _propertyName, List<ValidationResult> validationResults)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            if (!DateTime.TryParse(text, out var boolean))
            {
                validationResults.Add(new ValidationResult($"Property: {_propertyName} | Value: {text}, Error when trying to convert value: {text} to DateTime"));
                return false;
            }
            else
                return true;
        }
    }
}
