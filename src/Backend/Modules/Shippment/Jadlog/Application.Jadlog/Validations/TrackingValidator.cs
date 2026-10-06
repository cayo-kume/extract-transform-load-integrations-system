using Domain.Core.Extensions;
using Domain.Jadlog.DTOs;
using FluentValidation;
using System.Globalization;

namespace Domain.Jadlog.CustomValidations
{
    public class TrackingValidator : AbstractValidator<Tracking>
    {
        public TrackingValidator()
        {
            RuleFor(x => x.codigo)
                .TruncateIfExceeds(14, (x, val) => x.codigo = val);

            RuleFor(x => x.shipmentId)
                .TruncateIfExceeds(14, (x, val) => x.shipmentId = val);

            RuleFor(x => x.dacte)
                .TruncateIfExceeds(44, (x, val) => x.dacte = val);

            RuleFor(x => x.status)
                .TruncateIfExceeds(30, (x, val) => x.status = val);

            RuleFor(x => x.dtEmissao)
                .Must((x, dtEmissao) =>
                {
                    if (dtEmissao == null || DateTime.Parse(dtEmissao, new CultureInfo("pt-BR")) < new DateTime(1753, 1, 1))
                        x.dtEmissao = new DateTime(1753, 1, 1).ToString();
                    return true;
                })
                .WithMessage(x => $"Date: {x.dtEmissao} must be between 01/01/1753 and 31/12/9999.")
                .Must(x => ConvertToDateTimeValidation.IsValid(x))
                .WithMessage(x => $"Property: dtEmissao | Value: {x.dtEmissao}, Error when trying to convert value: {x.dtEmissao} to DateTime");

            RuleFor(x => x.recebedor)
                .SetValidator(new RecebedorValidator());

            RuleForEach(x => x.eventos)
                .SetValidator(new EventoValidator());

            RuleFor(x => x.volumes)
                .Custom((volumes, context) =>
                {
                    if (volumes == null)
                        context.InstanceToValidate.volumes = new List<Volume>();
                });

            RuleForEach(x => x.volumes)
                .SetValidator(new VolumeValidator());
        }
    }
}
