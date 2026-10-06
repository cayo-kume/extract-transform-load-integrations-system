using Domain.Jadlog.DTOs;
using FluentValidation;

namespace Domain.Jadlog.CustomValidations
{
    public class RootValidator : AbstractValidator<Root>
    {
        public RootValidator()
        {
            RuleForEach(x => x.consulta)
                .SetValidator(new ConsultaValidator());
        }
    }
}
