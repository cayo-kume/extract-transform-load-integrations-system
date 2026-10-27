using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Erp.Stores
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script004_Create_LinxMicrovix_Stores_Tables
    /// </summary>
    public class LinxGrupoLojasValidator : AbstractValidator<LinxGrupoLojas>
    {
        public LinxGrupoLojasValidator()
        {
            RuleFor(x => x.CNPJ)
                .MaximumLength(14)
                .WithMessage("O campo 'CNPJ' deve ter no máximo 14 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.CNPJ));

            RuleFor(x => x.nome_empresa)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_empresa' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_empresa));

            RuleFor(x => x.id_empresas_rede)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_empresas_rede));

            RuleFor(x => x.rede)
                .MaximumLength(100)
                .WithMessage("O campo 'rede' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rede));

            RuleFor(x => x.portal)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'portal' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.nome_portal)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_portal' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_portal));

            RuleFor(x => x.empresa)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'empresa' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.lojas_proprias)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.lojas_proprias));

            RuleFor(x => x.classificacao_portal)
                .MaximumLength(50)
                .WithMessage("O campo 'classificacao_portal' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.classificacao_portal));

            RuleFor(x => x.emp_mod_linx_promo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.emp_mod_linx_promo));
        }
    }
}
