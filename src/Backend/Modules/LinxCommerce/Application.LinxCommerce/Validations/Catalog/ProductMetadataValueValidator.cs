using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Catalog;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Catalog
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceProductsMetadataValues (scripts de tabelas do módulo)
    /// </summary>
    public class ProductMetadataValueValidator : AbstractValidator<ProductMetadataValue>
    {
        public ProductMetadataValueValidator()
        {
            RuleFor(x => x.PropertyMetadataID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'PropertyMetadataID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'PropertyMetadataID' deve ser um número inteiro.");

            RuleFor(x => x.PropertyName)
                .MaximumLength(250)
                .WithMessage("O campo 'PropertyName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.PropertyName));

            RuleFor(x => x.DisplayName)
                .MaximumLength(250)
                .WithMessage("O campo 'DisplayName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.DisplayName));

            RuleFor(x => x.PropertyGroup)
                .MaximumLength(250)
                .WithMessage("O campo 'PropertyGroup' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.PropertyGroup));
        }
    }
}
