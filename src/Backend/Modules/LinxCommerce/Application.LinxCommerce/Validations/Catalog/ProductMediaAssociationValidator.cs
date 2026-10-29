using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Catalog;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Catalog
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceProductsMediaAssociations (scripts de tabelas do módulo)
    /// </summary>
    public class ProductMediaAssociationValidator : AbstractValidator<ProductMediaAssociation>
    {
        public ProductMediaAssociationValidator()
        {
            RuleFor(x => x.ID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'ID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'ID' deve ser um número inteiro.");

            RuleFor(x => x.MediaID)
                .MustBeValidInt32()
                .WithMessage("O campo 'MediaID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.MediaID));

            RuleFor(x => x.Path)
                .MaximumLength(1000)
                .WithMessage("O campo 'Path' deve ter no máximo 1000 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Path));

            RuleFor(x => x.Order)
                .MustBeValidInt32()
                .WithMessage("O campo 'Order' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.Order));
        }
    }
}
