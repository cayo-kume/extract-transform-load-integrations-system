using Domain.Shared.Extensions;
using Domain.Totvs.Dtos.Reads.SalesOrders;
using FluentValidation;

namespace Application.Totvs.Validations.Reads.SalesOrders
{
    /// <summary>
    /// só registra mensagens no log; tamanhos do swagger da API Sales Order (colunas do script de tabelas de pedido de vendas)
    /// </summary>
    public class ClassificationValidator : AbstractValidator<Classification>
    {
        public ClassificationValidator()
        {
            RuleFor(x => x.typeCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'typeCode' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.code)
                .MaximumLength(10)
                .WithMessage("O campo 'code' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.code));

            RuleFor(x => x.typeName)
                .MaximumLength(40)
                .WithMessage("O campo 'typeName' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.typeName));

            RuleFor(x => x.name)
                .MaximumLength(40)
                .WithMessage("O campo 'name' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.name));
        }
    }
}
