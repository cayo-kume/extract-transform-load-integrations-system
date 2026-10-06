using System.Globalization;

namespace Domain.Jadlog.CustomValidations
{
    public static class ConvertToDateTimeValidation
    {
        public static bool IsValid(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            if (!DateTime.TryParse(text, new CultureInfo("pt-BR"), out var boolean))
                return false;
            else
                return true;
        }
    }
}
