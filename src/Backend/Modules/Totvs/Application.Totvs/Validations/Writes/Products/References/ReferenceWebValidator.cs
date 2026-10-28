using Application.Totvs.Services;
using Application.Totvs.Validations.Writes.People.Customers;
using Application.Totvs.Validations.Writes.Products.BarCodes;
using Application.Totvs.Validations.Writes.Products.Classifications;
using Domain.Totvs.Dtos.Writes.Products.References;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.Products.References
{
    /// <summary>
    /// dados web da referência: situação obrigatória, descrição (255), medidas com 9 inteiros e 3 decimais e descritivos com o tipo
    /// </summary>
    public class ReferenceWebValidator : AbstractValidator<ReferenceWeb>
    {
        public ReferenceWebValidator()
        {
            RuleFor(x => x.status)
                .NotNull()
                .WithMessage("O campo 'referenceWeb.status' é obrigatório (1 ativo, 2 bloqueado).")
                .IsInEnum()
                .WithMessage("O campo 'referenceWeb.status' só aceita 1 (ativo) ou 2 (bloqueado).");

            RuleFor(x => x.description)
                .MaximumLength(255)
                .WithMessage("O campo 'referenceWeb.description' aceita até 255 caracteres.");

            foreach (var (field, get) in new (string, Func<ReferenceWeb, decimal?>)[]
            {
                ("weight", x => x.weight), ("height", x => x.height), ("width", x => x.width), ("length", x => x.length),
                ("packWeight", x => x.packWeight), ("packHeight", x => x.packHeight), ("packWidth", x => x.packWidth), ("packLength", x => x.packLength)
            })
                RuleFor(x => get(x))
                    .InclusiveBetween(0, 999_999_999.999m)
                    .WithName(field)
                    .WithMessage($"O campo 'referenceWeb.{field}' aceita até 9 inteiros.")
                    .PrecisionScale(12, 3, true)
                    .WithMessage($"O campo 'referenceWeb.{field}' aceita até 3 casas decimais.")
                    .When(x => get(x) is not null);

            RuleForEach(x => x.descriptiveList)
                .ChildRules(d =>
                    d.RuleFor(x => x.type)
                        .NotNull()
                        .WithMessage("O campo 'type' do descritivo é obrigatório.")
                        .InclusiveBetween(1, 99)
                        .WithMessage("O campo 'type' do descritivo deve ter de 1 a 2 dígitos."))
                .When(x => x.descriptiveList is not null);
        }
    }
}
