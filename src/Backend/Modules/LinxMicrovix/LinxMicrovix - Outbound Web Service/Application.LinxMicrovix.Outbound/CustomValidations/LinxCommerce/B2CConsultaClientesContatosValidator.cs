using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaClientesContatosValidator : AbstractValidator<B2CConsultaClientesContatos>
    {
        public B2CConsultaClientesContatosValidator()
        {
            RuleFor(x => x.id_clientes_contatos)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_clientes_contatos));

            RuleFor(x => x.id_contato_b2c)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_contato_b2c));

            RuleFor(x => x.nome_contato)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_contato' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_contato));

            RuleFor(x => x.data_nasc_contato)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_nasc_contato));

            RuleFor(x => x.sexo_contato)
                .MaximumLength(1)
                .WithMessage("O campo 'sexo_contato' deve ter no máximo 1 caractere.")
                .When(x => !string.IsNullOrEmpty(x.sexo_contato));

            RuleFor(x => x.id_parentesco)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_parentesco));

            RuleFor(x => x.fone_contato)
                .MaximumLength(20)
                .WithMessage("O campo 'fone_contato' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fone_contato));

            RuleFor(x => x.celular_contato)
                .MaximumLength(20)
                .WithMessage("O campo 'celular_contato' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.celular_contato));

            RuleFor(x => x.email_contato)
                .MaximumLength(50)
                .WithMessage("O campo 'email_contato' deve ter no máximo 50 caracteres.")
                .EmailAddress()
                .When(x => !string.IsNullOrEmpty(x.email_contato));

            RuleFor(x => x.cod_cliente_erp)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_cliente_erp));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
