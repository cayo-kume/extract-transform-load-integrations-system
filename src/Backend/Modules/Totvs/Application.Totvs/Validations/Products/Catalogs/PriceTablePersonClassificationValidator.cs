using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script032_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class PriceTablePersonClassificationValidator : AbstractValidator<PriceTablePersonClassification>
    {
        public PriceTablePersonClassificationValidator()
        {
            RuleFor(x => x.typeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'typeCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.typeName)
                .MaximumLength(40)
                .WithMessage("O campo 'typeName' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeName));

            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MaximumLength(10)
                .WithMessage("O campo 'code' deve ter no máximo 10 caracteres.");

            RuleFor(x => x.name)
                .MaximumLength(60)
                .WithMessage("O campo 'name' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));
        }
    }
}
