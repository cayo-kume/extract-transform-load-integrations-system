using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaEmpresasValidator : AbstractValidator<B2CConsultaEmpresas>
    {
        public B2CConsultaEmpresasValidator()
        {
            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.nome_emp)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_emp' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_emp));

            RuleFor(x => x.cnpj_emp)
                .MaximumLength(14)
                .WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cnpj_emp));

            RuleFor(x => x.end_unidade)
                .MaximumLength(250)
                .WithMessage("O campo 'end_unidade' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.end_unidade));

            RuleFor(x => x.complemento_end_unidade)
                .MaximumLength(60)
                .WithMessage("O campo 'complemento_end_unidade' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.complemento_end_unidade));

            RuleFor(x => x.nr_rua_unidade)
                .MaximumLength(20)
                .WithMessage("O campo 'nr_rua_unidade' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nr_rua_unidade));

            RuleFor(x => x.bairro_unidade)
                .MaximumLength(60)
                .WithMessage("O campo 'bairro_unidade' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bairro_unidade));

            RuleFor(x => x.cep_unidade)
                .MaximumLength(9)
                .WithMessage("O campo 'cep_unidade' deve ter no máximo 9 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cep_unidade));

            RuleFor(x => x.cidade_unidade)
                .MaximumLength(50)
                .WithMessage("O campo 'cidade_unidade' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cidade_unidade));

            RuleFor(x => x.uf_unidade)
                .MaximumLength(2)
                .WithMessage("O campo 'uf_unidade' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.uf_unidade));

            RuleFor(x => x.email_unidade)
                .MaximumLength(50)
                .WithMessage("O campo 'email_unidade' deve ter no máximo 50 caracteres.")
                .EmailAddress()
                .When(x => !string.IsNullOrEmpty(x.email_unidade));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.data_criacao)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.data_criacao));

            RuleFor(x => x.centro_distribuicao)
                .MustBeValidBoolean()
                .When(x => !string.IsNullOrEmpty(x.centro_distribuicao));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
