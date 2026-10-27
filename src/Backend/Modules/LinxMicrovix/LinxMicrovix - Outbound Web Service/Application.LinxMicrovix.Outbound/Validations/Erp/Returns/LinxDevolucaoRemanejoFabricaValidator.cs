using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Returns;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Returns
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script046_Create_LinxMicrovix_Returns_Tables
    /// </summary>
    public class LinxDevolucaoRemanejoFabricaValidator : AbstractValidator<LinxDevolucaoRemanejoFabrica>
    {
        public LinxDevolucaoRemanejoFabricaValidator()
        {
            RuleFor(x => x.id_devolucao_remanejo_fabrica)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_devolucao_remanejo_fabrica' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.id_devolucao_remanejo_fabrica_tipo)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_devolucao_remanejo_fabrica_tipo));

            RuleFor(x => x.id_motivo_devolucao_fabrica)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_motivo_devolucao_fabrica));

            RuleFor(x => x.id_deposito)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_deposito));

            RuleFor(x => x.id_devolucao_remanejo_fabrica_status)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_devolucao_remanejo_fabrica_status));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.fornecedor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.fornecedor));

            RuleFor(x => x.cfop)
                .MaximumLength(10)
                .WithMessage("O campo 'cfop' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cfop));

            RuleFor(x => x.serie)
                .MaximumLength(10)
                .WithMessage("O campo 'serie' deve ter no máximo 10 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.serie));

            RuleFor(x => x.codigo_solicitacao)
                .MaximumLength(50)
                .WithMessage("O campo 'codigo_solicitacao' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.codigo_solicitacao));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.data_solicitacao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_solicitacao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal_2)
                .MaximumLength(1000)
                .WithMessage("O campo 'portal_2' deve ter no máximo 1000 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.portal_2));
        }
    }
}
