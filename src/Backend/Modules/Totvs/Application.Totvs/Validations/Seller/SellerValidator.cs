using FluentValidation;
using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Seller;

namespace Application.Totvs.Validations.Seller
{
    public class SellerValidator : AbstractValidator<Item>
    {
        public SellerValidator()
        {
            RuleFor(x => x.sellerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sellerCode));

            RuleFor(x => x.personCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personCode));

            RuleFor(x => x.maxChangeFilterDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.maxChangeFilterDate));

            RuleFor(x => x.sellerName)
                .MaximumLength(250)
                .WithMessage("O campo 'sellerName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sellerName));

            RuleFor(x => x.personName)
                .MaximumLength(250)
                .WithMessage("O campo 'personName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personName));

            RuleForEach(x => x.customers)
                .SetValidator(new CustomerValidator());

            RuleForEach(x => x.branchInformations)
                .SetValidator(new BranchInformationValidator());
        }
    }
}
