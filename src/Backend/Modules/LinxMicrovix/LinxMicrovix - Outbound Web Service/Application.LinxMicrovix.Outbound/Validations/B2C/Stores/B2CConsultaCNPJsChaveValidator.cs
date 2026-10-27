using Domain.LinxMicrovix.Outbound.Dtos.Reads.B2C.Stores;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.B2C.Stores
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da V64 e das colunas do Script007_Create_LinxMicrovix_B2C_Stores_Tables
    /// </summary>
    public class B2CConsultaCNPJsChaveValidator : AbstractValidator<B2CConsultaCNPJsChave>
    {
        public B2CConsultaCNPJsChaveValidator()
        {
            RuleFor(x => x.CNPJ)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'CNPJ' é obrigatório.")
                .MaximumLength(14)
                .WithMessage("O campo 'CNPJ' deve ter no máximo 14 caracteres.");

            RuleFor(x => x.nome_empresa)
                .MaximumLength(300)
                .WithMessage("O campo 'nome_empresa' deve ter no máximo 300 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_empresa));

            RuleFor(x => x.id_empresas_rede)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.id_empresas_rede));

            RuleFor(x => x.rede)
                .MaximumLength(100)
                .WithMessage("O campo 'rede' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.rede));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.nome_portal)
                .MaximumLength(50)
                .WithMessage("O campo 'nome_portal' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome_portal));

            RuleFor(x => x.empresa)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.empresa));

            RuleFor(x => x.classificacao_portal)
                .MaximumLength(50)
                .WithMessage("O campo 'classificacao_portal' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.classificacao_portal));

            RuleFor(x => x.b2c)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.b2c));

            RuleFor(x => x.oms)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.oms));

            RuleFor(x => x.b2c_marketplace)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.b2c_marketplace));
        }
    }
}
