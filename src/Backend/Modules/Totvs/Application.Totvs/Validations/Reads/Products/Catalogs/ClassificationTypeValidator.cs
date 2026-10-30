using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script032_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class ClassificationTypeValidator : AbstractValidator<ClassificationType>
    {
        public ClassificationTypeValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(40)
                .WithMessage("O campo 'description' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.auxiliaryDescription)
                .MaximumLength(100)
                .WithMessage("O campo 'auxiliaryDescription' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.auxiliaryDescription));

            RuleFor(x => x.isGroup)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.isGroup));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
