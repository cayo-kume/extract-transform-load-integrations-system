namespace Domain.LinxCommerce.CustomValidations.Validations
{
    public static class ConvertToDateTimeValidation
    {
        public static bool IsValid(string text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            if (!DateTime.TryParse(text, out var boolean))
                return false;
            else
                return true;
        }
    }
}
