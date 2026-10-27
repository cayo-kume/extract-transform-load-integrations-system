using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Returns
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script046_Create_LinxMicrovix_Returns_Tables
    /// </summary>
    public class LinxDevolucaoRemanejoFabricaTipoValidator : AbstractValidator<LinxDevolucaoRemanejoFabricaTipo>
    {
        public LinxDevolucaoRemanejoFabricaTipoValidator()
        {
            RuleFor(x => x.id_devolucao_remanejo_fabrica_tipo)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_devolucao_remanejo_fabrica_tipo' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao)
                .MaximumLength(100)
                .WithMessage("O campo 'descricao' deve ter no máximo 100 caracteres.")
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
