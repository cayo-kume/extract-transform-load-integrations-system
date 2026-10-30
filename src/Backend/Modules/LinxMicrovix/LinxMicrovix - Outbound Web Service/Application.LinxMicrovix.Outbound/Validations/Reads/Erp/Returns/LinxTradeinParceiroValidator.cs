using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Returns
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script046_Create_LinxMicrovix_Returns_Tables
    /// </summary>
    public class LinxTradeinParceiroValidator : AbstractValidator<LinxTradeinParceiro>
    {
        public LinxTradeinParceiroValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_tradein_parceiro)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_tradein_parceiro' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_parceiro)
                .MaximumLength(100)
                .WithMessage("O campo 'nome_parceiro' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_parceiro));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
