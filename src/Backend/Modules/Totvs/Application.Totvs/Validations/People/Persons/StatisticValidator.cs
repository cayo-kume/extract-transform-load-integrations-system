using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.Persons;
using FluentValidation;

namespace Application.Totvs.Validations.People.Persons
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Person (colunas do Script013_Create_Totvs_People_Tables)
    /// </summary>
    public class StatisticValidator : AbstractValidator<Statistic>
    {
        public StatisticValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.customerCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'customerCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.averageDelay)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.averageDelay));

            RuleFor(x => x.maximumDelay)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.maximumDelay));

            RuleFor(x => x.returnedCheck)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.returnedCheck));

            RuleFor(x => x.installmentPaid)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.installmentPaid));

            RuleFor(x => x.purchaseQuantity)
                .MustBeValidInt32()
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

            RuleFor(x => x.totalPurchase)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.totalPurchase));
        }
    }
}
