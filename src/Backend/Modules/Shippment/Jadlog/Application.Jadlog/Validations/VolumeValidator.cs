using Domain.Jadlog.DTOs;
using FluentValidation;

namespace Domain.Jadlog.CustomValidations
{
    public class VolumeValidator : AbstractValidator<Volume>
    {
        public VolumeValidator()
        {
            RuleFor(x => x.peso)
                .Must((x, peso) =>
                {
                    if (peso == null)
                        x.peso = 0;
                    return true;
                });

            RuleFor(x => x.altura)
                .Must((x, altura) =>
                {
                    if (altura == null)
                        x.altura = 0;
                    return true;
                });

            RuleFor(x => x.largura)
                .Must((x, largura) =>
                {
                    if (largura == null)
                        x.largura = 0;
                    return true;
                });

            RuleFor(x => x.comprimento)
                .Must((x, comprimento) =>
                {
                    if (comprimento == null)
                        x.comprimento = 0;
                    return true;
                });
        }
    }
}
