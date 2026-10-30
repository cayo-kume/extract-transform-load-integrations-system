using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Sellers;
using FluentValidation;

namespace Application.Totvs.Validations.Sellers
{
    /// <summary>
    /// só registra mensagens no log; tamanhos das colunas do Script008_Create_Totvs_Sellers_Tables
    /// </summary>
    public class BranchInformationValidator : AbstractValidator<BranchInformation>
    {
        public BranchInformationValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.auxiliaryCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.auxiliaryCode));

            RuleFor(x => x.isInactive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isInactive));

            RuleFor(x => x.sellerTypeCode)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.sellerTypeCode));

            RuleFor(x => x.sellerTypeDescription)
                .MaximumLength(60)
                .WithMessage("O campo 'sellerTypeDescription' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.sellerTypeDescription));
        }
    }
}
