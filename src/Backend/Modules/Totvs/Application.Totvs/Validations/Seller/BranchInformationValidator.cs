using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Seller;
using FluentValidation;

namespace Application.Totvs.Validations.Seller
{
    public class BranchInformationValidator : AbstractValidator<BranchInformation>
    {
        public BranchInformationValidator()
        {
            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.auxiliaryCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.auxiliaryCode));

            RuleFor(x => x.sellerTypeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sellerTypeCode));

            RuleFor(x => x.isInactive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isInactive));

            RuleFor(x => x.sellerTypeDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'sellerTypeDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sellerTypeDescription));
        }
    }
}
