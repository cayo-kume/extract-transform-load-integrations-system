using Domain.Totvs.Dtos.Product;
using FluentValidation;

namespace Application.Totvs.Validations.Product
{
    public class MedidaValidator : AbstractValidator<Medida>
    {
        public MedidaValidator()
        {
            RuleFor(x => x.chave)
                .MaximumLength(60)
                .WithMessage("O campo 'chave' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.chave));

            RuleFor(x => x.valor)
                .MaximumLength(60)
                .WithMessage("O campo 'valor' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.valor));
        }
    }
}
