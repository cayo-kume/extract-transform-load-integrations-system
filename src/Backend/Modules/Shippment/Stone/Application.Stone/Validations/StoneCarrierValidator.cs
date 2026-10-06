using FluentValidation;

namespace Application.Stone.Validations
{
    public class StoneCarrierValidator : AbstractValidator<Domain.Stone.Dtos.Carrier>
    {
        public StoneCarrierValidator()
        {
            RuleFor(x => x.name).MaximumLength(60).WithMessage("O campo 'name' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.name));
            RuleFor(x => x.service).MaximumLength(60).WithMessage("O campo 'service' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.service));
        }
    }
}
