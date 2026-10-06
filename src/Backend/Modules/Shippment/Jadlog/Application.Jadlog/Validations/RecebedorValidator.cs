using Domain.Core.Extensions;
using Domain.Jadlog.DTOs;
using FluentValidation;

namespace Domain.Jadlog.CustomValidations
{
    public class RecebedorValidator : AbstractValidator<Recebedor>
    {
        public RecebedorValidator()
        {
            RuleFor(x => x.nome)
                .TruncateIfExceeds(60, (x, val) => x.nome = val);

            RuleFor(x => x.doc)
                .TruncateIfExceeds(14, (x, val) => x.doc = val);
        }
    }
}
