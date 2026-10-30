using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Sellers;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Sellers
{
    /// <summary>
    /// só registra mensagens no log; o swagger da API Seller não traz tamanhos, então valem os das colunas do
    /// Script008_Create_Totvs_Sellers_Tables (os mesmos do código antigo)
    /// </summary>
    public class SellerValidator : AbstractValidator<Seller>
    {
        public SellerValidator()
        {
            RuleFor(x => x.sellerCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'sellerCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.sellerName)
                .MaximumLength(250)
                .WithMessage("O campo 'sellerName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sellerName));

            RuleFor(x => x.personCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.personCode));

            RuleFor(x => x.personName)
                .MaximumLength(250)
                .WithMessage("O campo 'personName' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.personName));

            // não anulável no swagger; sem ele o vendedor é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();

            RuleForEach(x => x.branchInformations)
                .SetValidator(new BranchInformationValidator());

            RuleForEach(x => x.customers)
                .SetValidator(new CustomerValidator());
        }
    }
}
