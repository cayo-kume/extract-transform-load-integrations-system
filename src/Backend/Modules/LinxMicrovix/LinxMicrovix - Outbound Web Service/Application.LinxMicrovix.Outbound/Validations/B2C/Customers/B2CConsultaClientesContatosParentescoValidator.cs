using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.Customers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script070_Create_LinxMicrovix_B2C_Customers_Tables
    /// </summary>
    public class B2CConsultaClientesContatosParentescoValidator : AbstractValidator<B2CConsultaClientesContatosParentesco>
    {
        public B2CConsultaClientesContatosParentescoValidator()
        {
            RuleFor(x => x.id_parentesco)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_parentesco' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
