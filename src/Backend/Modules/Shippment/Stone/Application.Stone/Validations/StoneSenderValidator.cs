using Domain.Core.Extensions;
using FluentValidation;

namespace Application.Stone.Validations
{
    public class StoneSenderValidator : AbstractValidator<Domain.Stone.Dtos.Sender>
    {
        public StoneSenderValidator()
        {
            RuleFor(x => x.logisticAccountId).MustBeValidGuid().When(x => !string.IsNullOrEmpty(x.logisticAccountId));
            RuleFor(x => x.name).MaximumLength(60).WithMessage("O campo 'name' deve ter no máximo 60 caractere.").When(x => !string.IsNullOrEmpty(x.name));
            RuleFor(x => x.email).MaximumLength(50).WithMessage("O campo 'email' deve ter no máximo 50 caractere.").When(x => !string.IsNullOrEmpty(x.email));
            RuleFor(x => x.document).MaximumLength(14).WithMessage("O campo 'document' deve ter no máximo 14 caractere.").When(x => !string.IsNullOrEmpty(x.document));
            RuleFor(x => x.phoneNumber).MaximumLength(20).WithMessage("O campo 'phoneNumber' deve ter no máximo 20 caractere.").When(x => !string.IsNullOrEmpty(x.phoneNumber));
        }
    }
}
