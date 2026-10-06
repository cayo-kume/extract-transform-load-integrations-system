using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxCommerce
{
    public class B2CConsultaFornecedoresValidator : AbstractValidator<B2CConsultaFornecedores>
    {
        public B2CConsultaFornecedoresValidator()
        {
            RuleFor(x => x.cod_fornecedor)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_fornecedor));

            RuleFor(x => x.nome)
                .MaximumLength(60)
                .WithMessage("O campo 'nome' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome));

            RuleFor(x => x.nome_fantasia)
                .MaximumLength(60)
                .WithMessage("O campo 'nome_fantasia' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_fantasia));

            RuleFor(x => x.tipo_pessoa)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_pessoa' deve ter no máximo 1 caractere.")
                .When(x => !string.IsNullOrEmpty(x.tipo_pessoa));

            RuleFor(x => x.tipo_fornecedor)
                .MaximumLength(1)
                .WithMessage("O campo 'tipo_fornecedor' deve ter no máximo 1 caractere.")
                .When(x => !string.IsNullOrEmpty(x.tipo_fornecedor));

            RuleFor(x => x.endereco)
                .MaximumLength(250)
                .WithMessage("O campo 'endereco' deve ter no máximo 250 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.endereco));

            RuleFor(x => x.numero_rua)
                .MaximumLength(20)
                .WithMessage("O campo 'numero_rua' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.numero_rua));

            RuleFor(x => x.bairro)
                .MaximumLength(60)
                .WithMessage("O campo 'bairro' deve ter no máximo 60 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.bairro));

            RuleFor(x => x.cep)
                .MaximumLength(9)
                .WithMessage("O campo 'cep' deve ter no máximo 9 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cep));

            RuleFor(x => x.cidade)
                .MaximumLength(40)
                .WithMessage("O campo 'cidade' deve ter no máximo 40 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.cidade));

            RuleFor(x => x.uf)
                .MaximumLength(2)
                .WithMessage("O campo 'uf' deve ter no máximo 2 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.uf));

            RuleFor(x => x.documento)
                .MaximumLength(14)
                .WithMessage("O campo 'documento' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.documento));

            RuleFor(x => x.fone)
                .MaximumLength(20)
                .WithMessage("O campo 'fone' deve ter no máximo 20 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.fone));

            RuleFor(x => x.email)
                .MaximumLength(50)
                .WithMessage("O campo 'email' deve ter no máximo 50 caracteres.")
                .EmailAddress()
                .When(x => !string.IsNullOrEmpty(x.email));

            RuleFor(x => x.pais)
                .MaximumLength(80)
                .WithMessage("O campo 'pais' deve ter no máximo 80 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.pais));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
