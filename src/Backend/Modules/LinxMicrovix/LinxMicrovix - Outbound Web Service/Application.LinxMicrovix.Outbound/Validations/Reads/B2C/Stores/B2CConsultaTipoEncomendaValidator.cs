using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.B2C.Stores
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script007_Create_LinxMicrovix_B2C_Stores_Tables
    /// </summary>
    public class B2CConsultaTipoEncomendaValidator : AbstractValidator<B2CConsultaTipoEncomenda>
    {
        public B2CConsultaTipoEncomendaValidator()
        {
            RuleFor(x => x.id_tipo_encomenda)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_tipo_encomenda' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nm_tipo_encomenda)
                .MaximumLength(100)
                .WithMessage("O campo 'nm_tipo_encomenda' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nm_tipo_encomenda));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
