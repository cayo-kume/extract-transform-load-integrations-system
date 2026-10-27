using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Payments;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Payments
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script034_Create_LinxMicrovix_Payments_Tables
    /// </summary>
    public class LinxValesComprasEnviadosAPIValidator : AbstractValidator<LinxValesComprasEnviadosAPI>
    {
        public LinxValesComprasEnviadosAPIValidator()
        {
            RuleFor(x => x.numero_controle)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'numero_controle' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.valor_vale)
                .MustBeValidDecimal()
                .When(x => !string.IsNullOrEmpty(x.valor_vale));

            RuleFor(x => x.doc_cliente)
                .MaximumLength(14)
                .WithMessage("O campo 'doc_cliente' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.doc_cliente));

            RuleFor(x => x.status_vale)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.status_vale));

            RuleFor(x => x.codigo_portal_resgate)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_portal_resgate));

            RuleFor(x => x.codigo_empresa_resgate)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_empresa_resgate));

            RuleFor(x => x.codigo_usuario_resgate)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_usuario_resgate));

            RuleFor(x => x.codigo_vale_empresa_resgate)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.codigo_vale_empresa_resgate));

            RuleFor(x => x.data_criacao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_criacao));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.cnpj_empresa_resgate)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_empresa_resgate' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_empresa_resgate));

            RuleFor(x => x.data_resgate)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_resgate));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
