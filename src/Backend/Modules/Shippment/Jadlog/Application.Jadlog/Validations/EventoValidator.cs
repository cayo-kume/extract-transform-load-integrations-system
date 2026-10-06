using Domain.Core.Extensions;
using Domain.Jadlog.DTOs;
using FluentValidation;
using System.Globalization;

namespace Domain.Jadlog.CustomValidations
{
    public class EventoValidator : AbstractValidator<Evento>
    {
        public EventoValidator()
        {
            RuleFor(x => x.status)
                .TruncateIfExceeds(30, (x, val) => x.status = val);

            RuleFor(x => x.unidade)
                .TruncateIfExceeds(50, (x, val) => x.unidade = val);

            RuleFor(x => x.data)
                .Must((x, data) =>
                {
                    if (data == null || DateTime.Parse(data, new CultureInfo("pt-BR")) < new DateTime(1753, 1, 1))
                        x.data = new DateTime(1753, 1, 1).ToString();
                    return true;
                })
                .WithMessage(x => $"Date: {x.data} must be between 01/01/1753 and 31/12/9999.")
                .Must(x => ConvertToDateTimeValidation.IsValid(x.ToString()))
                .WithMessage(x => $"Property: data | Value: {x.data}, Error when trying to convert value: {x.data.ToString()} to DateTime");
        }
    }
}
