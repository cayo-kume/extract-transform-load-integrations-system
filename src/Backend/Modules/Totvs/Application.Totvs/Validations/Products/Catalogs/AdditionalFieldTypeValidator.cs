using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.Products.Catalogs;
using FluentValidation;

namespace Application.Totvs.Validations.Products.Catalogs
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Product e das colunas do Script032_Create_Totvs_Product_Catalogs_Tables
    /// </summary>
    public class AdditionalFieldTypeValidator : AbstractValidator<AdditionalFieldType>
    {
        public AdditionalFieldTypeValidator()
        {
            RuleFor(x => x.code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'code' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.description)
                .MaximumLength(60)
                .WithMessage("O campo 'description' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.description));

            RuleFor(x => x.title)
                .MaximumLength(30)
                .WithMessage("O campo 'title' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.title));

            RuleFor(x => x.type)
                .MaximumLength(30)
                .WithMessage("O campo 'type' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.type));

            RuleFor(x => x.size)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.size));

            RuleFor(x => x.@decimal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.@decimal));

            // sem ele o registro é regravado em toda execução
            RuleFor(x => x.maxChangeFilterDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'maxChangeFilterDate' veio vazio.")
                .MustBeValidDateTime();
        }
    }
}
