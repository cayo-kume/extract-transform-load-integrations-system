using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxLojasValidator : AbstractValidator<LinxLojas>
    {
        public LinxLojasValidator()
        {
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.empresa).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.empresa));
            RuleFor(x => x.nome_emp).MaximumLength(50).WithMessage("O campo 'nome_emp' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nome_emp));
            RuleFor(x => x.razao_emp).MaximumLength(200).WithMessage("O campo 'razao_emp' deve ter no máximo 200 caracteres.").When(x => !string.IsNullOrEmpty(x.razao_emp));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.inscricao_emp).MaximumLength(20).WithMessage("O campo 'inscricao_emp' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.inscricao_emp));
            RuleFor(x => x.endereco_emp).MaximumLength(250).WithMessage("O campo 'endereco_emp' deve ter no máximo 250 caracteres.").When(x => !string.IsNullOrEmpty(x.endereco_emp));
            RuleFor(x => x.num_emp).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.num_emp));
            RuleFor(x => x.complement_emp).MaximumLength(60).WithMessage("O campo 'complement_emp' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.complement_emp));
            RuleFor(x => x.bairro_emp).MaximumLength(50).WithMessage("O campo 'bairro_emp' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.bairro_emp));
            RuleFor(x => x.cep_emp).MaximumLength(9).WithMessage("O campo 'cep_emp' deve ter no máximo 9 caracteres.").When(x => !string.IsNullOrEmpty(x.cep_emp));
            RuleFor(x => x.cidade_emp).MaximumLength(50).WithMessage("O campo 'cidade_emp' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.cidade_emp));
            RuleFor(x => x.estado_emp).MaximumLength(2).WithMessage("O campo 'estado_emp' deve ter no máximo 2 caracteres.").When(x => !string.IsNullOrEmpty(x.estado_emp));
            RuleFor(x => x.fone_emp).MaximumLength(70).WithMessage("O campo 'fone_emp' deve ter no máximo 70 caracteres.").When(x => !string.IsNullOrEmpty(x.fone_emp));
            RuleFor(x => x.email_emp).MaximumLength(50).WithMessage("O campo 'email_emp' deve ter no máximo 50 caracteres.").EmailAddress().When(x => !string.IsNullOrEmpty(x.email_emp));
            RuleFor(x => x.cod_ibge_municipio).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.cod_ibge_municipio));
            RuleFor(x => x.data_criacao_emp).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_criacao_emp));
            RuleFor(x => x.data_criacao_portal).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_criacao_portal));
            RuleFor(x => x.sistema_tributacao).MaximumLength(1).WithMessage("O campo 'sistema_tributacao' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.sistema_tributacao));
            RuleFor(x => x.regime_tributario).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.regime_tributario));
            RuleFor(x => x.area_empresa).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.area_empresa));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.sigla_empresa).MaximumLength(10).WithMessage("O campo 'sigla_empresa' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.sigla_empresa));
            RuleFor(x => x.id_classe_fiscal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_classe_fiscal));
            RuleFor(x => x.centro_distribuicao).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.centro_distribuicao));
            RuleFor(x => x.inscricao_municipal_emp).MaximumLength(30).WithMessage("O campo 'inscricao_municipal_emp' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.inscricao_municipal_emp));
            RuleFor(x => x.cnae_emp).MaximumLength(7).WithMessage("O campo 'cnae_emp' deve ter no máximo 7 caracteres.").When(x => !string.IsNullOrEmpty(x.cnae_emp));
            RuleFor(x => x.cod_cliente_linx).MaximumLength(6).WithMessage("O campo 'cod_cliente_linx' deve ter no máximo 6 caracteres.").When(x => !string.IsNullOrEmpty(x.cod_cliente_linx));
            RuleFor(x => x.tabela_preco_preferencial).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.tabela_preco_preferencial));
        }
    }
}
