using Domain.Core.Extensions;
using FluentValidation;

namespace Application.Stone.Validations
{
    public class StoneEventValidator : AbstractValidator<Domain.Stone.Dtos.Event>
    {
        public StoneEventValidator()
        {
            RuleFor(x => x.id).MustBeValidGuid().When(x => !string.IsNullOrEmpty(x.id));
            RuleFor(x => x.createdAt).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.createdAt));
            RuleFor(x => x.status).MaximumLength(60).WithMessage("O campo 'status' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.status));
            RuleFor(x => x.modifiedBy).MaximumLength(60).WithMessage("O campo 'modifiedBy' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.modifiedBy));
            RuleFor(x => x.trackingCode).MaximumLength(60).WithMessage("O campo 'trackingCode' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.trackingCode));
            RuleFor(x => x.description).MaximumLength(60).WithMessage("O campo 'description' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.carrier)
                .SetValidator(new StoneCarrierValidator());
        }
    }
}
