using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Orders
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script088_Create_LinxMicrovix_B2C_Orders_Tables
    /// </summary>
    public class B2CConsultaPedidosTiposValidator : AbstractValidator<B2CConsultaPedidosTipos>
    {
        public B2CConsultaPedidosTiposValidator()
        {
            RuleFor(x => x.id_tipo_b2c)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_tipo_b2c' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(200)
                .WithMessage("O campo 'descricao' deve ter no máximo 200 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
