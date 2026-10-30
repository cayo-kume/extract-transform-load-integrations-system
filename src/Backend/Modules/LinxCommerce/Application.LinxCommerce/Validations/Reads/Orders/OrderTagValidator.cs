using Application.LinxCommerce.Validations.Reads.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Reads.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrdersTags (scripts de tabelas do módulo)
    /// </summary>
    public class OrderTagValidator : AbstractValidator<OrderTag>
    {
        public OrderTagValidator()
        {
            RuleFor(x => x.TagID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'TagID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'TagID' deve ser um número inteiro.");

            RuleFor(x => x.Alias)
                .MaximumLength(100)
                .WithMessage("O campo 'Alias' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Alias));

            RuleFor(x => x.Name)
                .MaximumLength(250)
                .WithMessage("O campo 'Name' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Name));

            RuleFor(x => x.IsSystem)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsSystem' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsSystem));

            RuleFor(x => x.IsDeleted)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsDeleted' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsDeleted));
        }
    }
}
