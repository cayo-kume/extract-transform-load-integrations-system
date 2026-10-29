using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Catalog;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Catalog
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceSkusParentRelations (scripts de tabelas do módulo)
    /// </summary>
    public class SkuParentRelationValidator : AbstractValidator<SkuParentRelation>
    {
        public SkuParentRelationValidator()
        {
            RuleFor(x => x.ParentID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'ParentID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'ParentID' deve ser um número inteiro.");

            RuleFor(x => x.ParentSKU)
                .MaximumLength(100)
                .WithMessage("O campo 'ParentSKU' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ParentSKU));

            RuleFor(x => x.ParentIntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'ParentIntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ParentIntegrationID));
        }
    }
}
