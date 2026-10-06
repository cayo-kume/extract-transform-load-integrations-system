using Domain.Totvs.Dtos.Person.Customer;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.Totvs.Validations.Person.Customer
{
    public class StatisticValidator : AbstractValidator<Statistic>
    {
        public StatisticValidator()
        {
            RuleFor(x => x.branchCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.branchCode));

            RuleFor(x => x.customerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.customerCode));

            RuleFor(x => x.averageDelay)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.averageDelay));

            RuleFor(x => x.maximumDelay)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.maximumDelay));

            RuleFor(x => x.returnedCheck)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.returnedCheck));

            RuleFor(x => x.installmentPaid)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.installmentPaid));

            RuleFor(x => x.purchaseQuantity)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.purchaseQuantity));

            RuleFor(x => x.higherDebt)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.higherDebt));

            RuleFor(x => x.limitValue)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.limitValue));

            RuleFor(x => x.biggestPurchase)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.biggestPurchase));

            RuleFor(x => x.firstPurchase)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.firstPurchase));

            RuleFor(x => x.lastPurchase)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.lastPurchase));

            RuleFor(x => x.totalPurchase)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.totalPurchase));

            RuleFor(x => x.largestAccumulationDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.largestAccumulationDate));

            RuleFor(x => x.biggestPurchaseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.biggestPurchaseDate));

            RuleFor(x => x.firstPurchaseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.firstPurchaseDate));

            RuleFor(x => x.lastPurchaseDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastPurchaseDate));

            RuleFor(x => x.lastCardDate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.lastCardDate));
        }
    }
}
