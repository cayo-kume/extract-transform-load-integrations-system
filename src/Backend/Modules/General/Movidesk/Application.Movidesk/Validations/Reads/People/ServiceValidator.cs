using Domain.Shared.Extensions;
using Domain.Movidesk.Dtos.Reads.People;
using FluentValidation;

namespace Application.Movidesk.Validations.Reads.People
{
    public class ServiceValidator : AbstractValidator<Service>
    {
        public ServiceValidator()
        {
            RuleFor(x => x.id)
               .MustBeValidInt32()
               .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.name)
                .MaximumLength(128)
                .WithMessage("O campo 'name' deve ter no máximo 128 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));

            RuleFor(x => x.copyToChildren)
               .MustBeValidBoolean()
               .When(x => !string.IsNullOrEmpty(x.copyToChildren));
        }
    }
}
