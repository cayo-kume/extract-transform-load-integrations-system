using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.ProductFields;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.ProductFields
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script082_Create_LinxMicrovix_B2C_Product_Fields_Tables
    /// </summary>
    public class B2CConsultaProdutosCamposAdicionaisNomesValidator : AbstractValidator<B2CConsultaProdutosCamposAdicionaisNomes>
    {
        public B2CConsultaProdutosCamposAdicionaisNomesValidator()
        {
            RuleFor(x => x.id_campo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_campo' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.ordem)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.ordem));

            RuleFor(x => x.legenda)
                .MaximumLength(30)
                .WithMessage("O campo 'legenda' deve ter no máximo 30 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.legenda));

            RuleFor(x => x.tipo)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo' deve ter no máximo 1 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.tipo));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
