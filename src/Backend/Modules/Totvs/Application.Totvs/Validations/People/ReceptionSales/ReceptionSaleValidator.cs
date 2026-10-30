using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.People.ReceptionSales;
using FluentValidation;

namespace Application.Totvs.Validations.People.ReceptionSales
{
    /// <summary>
    /// só registra mensagens no log; o swagger não dá tamanho dos nomes, do tipo nem da observação (colunas do
    /// Script029_Create_Totvs_Representatives_Tables)
    /// </summary>
    public class ReceptionSaleValidator : AbstractValidator<ReceptionSale>
    {
        public ReceptionSaleValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.movementDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'movementDate' é obrigatório.")
                .MustBeValidDateTime();

            RuleFor(x => x.sequence)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'sequence' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.customerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.customerCode));

            RuleFor(x => x.customerName)
                .MaximumLength(100)
                .WithMessage("O campo 'customerName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.customerName));

            RuleFor(x => x.sellerCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sellerCode));

            RuleFor(x => x.sellerName)
                .MaximumLength(100)
                .WithMessage("O campo 'sellerName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sellerName));

            RuleFor(x => x.representativeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.representativeCode));

            RuleFor(x => x.representativeName)
                .MaximumLength(100)
                .WithMessage("O campo 'representativeName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.representativeName));

            RuleFor(x => x.guideCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.guideCode));

            RuleFor(x => x.guideName)
                .MaximumLength(100)
                .WithMessage("O campo 'guideName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.guideName));

            RuleFor(x => x.receptionType)
                .MaximumLength(10)
                .WithMessage("O campo 'receptionType' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.receptionType));

            RuleFor(x => x.checkInTime)
                .MaximumLength(10)
                .WithMessage("O campo 'checkInTime' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.checkInTime));

            RuleFor(x => x.observation)
                .MaximumLength(400)
                .WithMessage("O campo 'observation' deve ter no máximo 400 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.observation));
        }
    }
}
