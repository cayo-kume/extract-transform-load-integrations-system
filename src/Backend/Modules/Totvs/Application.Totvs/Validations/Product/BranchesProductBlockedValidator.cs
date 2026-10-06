using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class BranchesProductBlockedValidator : AbstractValidator<BranchesProductBlocked>
    {
        public BranchesProductBlockedValidator()
        {
			RuleFor(x => x.branchCode)
				.MustBeValidInt32()
				.When(x => !string.IsNullOrEmpty(x.branchCode));

			RuleFor(x => x.isBlocked)
				.MustBeValidBoolean()
				.When(x => !string.IsNullOrEmpty(x.isBlocked));
		}
    }
}
