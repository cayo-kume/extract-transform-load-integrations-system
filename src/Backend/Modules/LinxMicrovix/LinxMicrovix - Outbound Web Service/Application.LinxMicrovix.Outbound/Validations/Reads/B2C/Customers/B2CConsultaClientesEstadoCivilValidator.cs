using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Customers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Customers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script070_Create_LinxMicrovix_B2C_Customers_Tables
    /// </summary>
    public class B2CConsultaClientesEstadoCivilValidator : AbstractValidator<B2CConsultaClientesEstadoCivil>
    {
        public B2CConsultaClientesEstadoCivilValidator()
        {
            RuleFor(x => x.id_estado_civil)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_estado_civil' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.estado_civil)
                .MaximumLength(20)
                .WithMessage("O campo 'estado_civil' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.estado_civil));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
