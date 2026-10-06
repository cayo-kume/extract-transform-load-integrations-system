using System.ComponentModel.DataAnnotations;

namespace Domain.LinxMicrovix.Outbound.WebService.CustomValidations
{
    public class LengthValidation : ValidationAttribute
    {
        private string _propertyName { get; set; }
        private int _length { get; set; }

        /// <param name="length"></param>
        /// <param name="propertyName"></param>
        public LengthValidation(int length, string propertyName) =>
            (_length, _propertyName) = (length, propertyName);

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string? text = value == null ? "" : value.ToString();

            return text.Length > _length
                ? new ValidationResult($"Property: {_propertyName} | Value: {text}, Tamanho do texto: {text.Length} excede ao permitido: {_length}")
                : ValidationResult.Success;
        }
    }
}
