using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxMovimentoCartoesValidator : AbstractValidator<LinxMovimentoCartoes>
    {
        public LinxMovimentoCartoesValidator()
        {
            RuleFor(x => x.identificador).Must(value => Guid.TryParse(value, out _)).WithMessage("O campo 'identificador' deve ser um GUID válido.").When(x => !string.IsNullOrEmpty(x.identificador));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.cnpj_emp).MaximumLength(14).WithMessage("O campo 'cnpj_emp' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.cnpj_emp));
            RuleFor(x => x.codlojasitef).MaximumLength(10).WithMessage("O campo 'codlojasitef' deve ter no máximo 10 caracteres.").When(x => !string.IsNullOrEmpty(x.codlojasitef));
            RuleFor(x => x.data_lancamento).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_lancamento));
            RuleFor(x => x.cupomfiscal).MaximumLength(20).WithMessage("O campo 'cupomfiscal' deve ter no máximo 20 caracteres.").When(x => !string.IsNullOrEmpty(x.cupomfiscal));
            RuleFor(x => x.credito_debito).MaximumLength(1).WithMessage("O campo 'credito_debito' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.credito_debito));
            RuleFor(x => x.id_cartao_bandeira).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_cartao_bandeira));
            RuleFor(x => x.descricao_bandeira).MaximumLength(100).WithMessage("O campo 'descricao_bandeira' deve ter no máximo 100 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_bandeira));
            RuleFor(x => x.valor).MustBeValidDecimal().When(x => !string.IsNullOrEmpty(x.valor));
            RuleFor(x => x.ordem_cartao).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.ordem_cartao));
            RuleFor(x => x.nsu_host).MaximumLength(50).WithMessage("O campo 'nsu_host' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nsu_host));
            RuleFor(x => x.nsu_sitef).MaximumLength(50).WithMessage("O campo 'nsu_sitef' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.nsu_sitef));
            RuleFor(x => x.cod_autorizacao).MaximumLength(50).WithMessage("O campo 'cod_autorizacao' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.cod_autorizacao));
            RuleFor(x => x.id_antecipacoes_financeiras).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_antecipacoes_financeiras));
            RuleFor(x => x.transacao_servico_terceiro).MustBeValidBoolean().When(x => !string.IsNullOrEmpty(x.transacao_servico_terceiro));
            RuleFor(x => x.id_maquineta_pos).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.id_maquineta_pos));
            RuleFor(x => x.descricao_maquineta).MaximumLength(50).WithMessage("O campo 'descricao_maquineta' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.descricao_maquineta));
            RuleFor(x => x.serie_maquineta).MaximumLength(50).WithMessage("O campo 'serie_maquineta' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.serie_maquineta));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
            RuleFor(x => x.cartao_prepago).MaximumLength(1).WithMessage("O campo 'cartao_prepago' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.cartao_prepago));
        }
    }
}
