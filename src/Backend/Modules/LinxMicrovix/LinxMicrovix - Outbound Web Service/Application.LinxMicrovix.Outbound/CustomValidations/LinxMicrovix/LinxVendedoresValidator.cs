using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxVendedoresValidator : AbstractValidator<LinxVendedores>
    {
        public LinxVendedoresValidator()
        {
            RuleFor(x => x.cod_vendedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_vendedor));
            RuleFor(x => x.nome_vendedor).MaximumLength(50).WithMessage("O campo 'nome_vendedor' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_vendedor));
            RuleFor(x => x.tipo_vendedor).MaximumLength(1).WithMessage("O campo 'tipo_vendedor' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.tipo_vendedor));
            RuleFor(x => x.end_vend_rua).MaximumLength(250).WithMessage("O campo 'end_vend_rua' deve ter no máximo 250 caracteres.").When(x => !string.IsNullOrEmpty(x.end_vend_rua));
            RuleFor(x => x.end_vend_numero).MaximumLength(20).WithMessage("O campo 'end_vend_numero' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.end_vend_numero));
            RuleFor(x => x.end_vend_complemento).MaximumLength(60).WithMessage("O campo 'end_vend_complemento' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.end_vend_complemento));
            RuleFor(x => x.end_vend_bairro).MaximumLength(60).WithMessage("O campo 'end_vend_bairro' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.end_vend_bairro));
            RuleFor(x => x.end_vend_cep).MaximumLength(9).WithMessage("O campo 'end_vend_cep' deve ter no máximo 9 caracteres.").When(x => !string.IsNullOrEmpty(x.end_vend_cep));
            RuleFor(x => x.end_vend_cidade).MaximumLength(40).WithMessage("O campo 'end_vend_cidade' deve ter no máximo 40 caracteres.").When(x => !string.IsNullOrEmpty(x.end_vend_cidade));
            RuleFor(x => x.end_vend_uf).MaximumLength(2).WithMessage("O campo 'end_vend_uf' deve ter no máximo 2 caracteres.").When(x => !string.IsNullOrEmpty(x.end_vend_uf));
            RuleFor(x => x.fone_vendedor).MaximumLength(20).WithMessage("O campo 'fone_vendedor' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.fone_vendedor));
            RuleFor(x => x.mail_vendedor).MaximumLength(50).WithMessage("O campo 'mail_vendedor' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.mail_vendedor));
            RuleFor(x => x.dt_upd).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.dt_upd));
            RuleFor(x => x.cpf_vendedor).MaximumLength(14).WithMessage("O campo 'cpf_vendedor' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cpf_vendedor));
            RuleFor(x => x.ativo).MaximumLength(1).WithMessage("O campo 'ativo' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.ativo));
            RuleFor(x => x.data_admissao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_admissao));
            RuleFor(x => x.data_saida).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_saida));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.matricula).MaximumLength(30).WithMessage("O campo 'matricula' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.matricula));
            RuleFor(x => x.id_tipo_venda).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_tipo_venda));
            RuleFor(x => x.descricao_tipo_venda).MaximumLength(100).WithMessage("O campo 'descricao_tipo_venda' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_tipo_venda));
            RuleFor(x => x.cargo).MaximumLength(20).WithMessage("O campo 'cargo' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.cargo));
        }
    }
}