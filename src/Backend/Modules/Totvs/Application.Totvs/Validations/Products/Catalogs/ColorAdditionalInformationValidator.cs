using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script045_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class ColorAdditionalInformationValidator : AbstractValidator<ColorAdditionalInformation>
    {
        public ColorAdditionalInformationValidator()
        {
            RuleFor(x => x.branchCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'branchCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.groupName)
                .MaximumLength(10)
                .WithMessage("O campo 'groupName' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.groupName));

            RuleFor(x => x.auxiliaryCode)
                .MaximumLength(10)
                .WithMessage("O campo 'auxiliaryCode' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.auxiliaryCode));

            RuleFor(x => x.auxiliaryName)
                .MaximumLength(80)
                .WithMessage("O campo 'auxiliaryName' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.auxiliaryName));

            RuleFor(x => x.pantoneCode)
                .MaximumLength(40)
                .WithMessage("O campo 'pantoneCode' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.pantoneCode));

            RuleFor(x => x.colorType)
                .MaximumLength(2)
                .WithMessage("O campo 'colorType' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.colorType));

            RuleFor(x => x.colorTypeName)
                .MaximumLength(40)
                .WithMessage("O campo 'colorTypeName' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.colorTypeName));

            RuleFor(x => x.isActive)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isActive));

            RuleFor(x => x.isFinishedProduct)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isFinishedProduct));

            RuleFor(x => x.isRawMaterial)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isRawMaterial));

            RuleFor(x => x.isBulkMaterial)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isBulkMaterial));
        }
    }
}
