using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxClientesFornecContatosValidator : AbstractValidator<LinxClientesFornecContatos>
    {
        public LinxClientesFornecContatosValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cod_cliente).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_cliente));
            RuleFor(x => x.nome_contato).MaximumLength(50).WithMessage("O campo 'nome_contato' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_contato));
            RuleFor(x => x.sexo_contato).MaximumLength(1).WithMessage("O campo 'sexo_contato' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.sexo_contato));
            RuleFor(x => x.contatos_clientes_parentesco).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.contatos_clientes_parentesco));
            RuleFor(x => x.fone1_contato).MaximumLength(20).WithMessage("O campo 'fone1_contato' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.fone1_contato));
            RuleFor(x => x.fone2_contato).MaximumLength(20).WithMessage("O campo 'fone2_contato' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.fone2_contato));
            RuleFor(x => x.celular_contato).MaximumLength(20).WithMessage("O campo 'celular_contato' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.celular_contato));
            RuleFor(x => x.email_contato).MaximumLength(50).WithMessage("O campo 'email_contato' deve ter no máximo 50 caracteres.").EmailAddress().When(x => !string.IsNullOrEmpty(x.email_contato));
            RuleFor(x => x.data_nasc_contato).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_nasc_contato));
            RuleFor(x => x.tipo_contato).MaximumLength(20).WithMessage("O campo 'tipo_contato' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.tipo_contato));
        }
    }
}
