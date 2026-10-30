using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Taxes;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Taxes
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script013_Create_LinxMicrovix_Taxes_Tables
    /// </summary>
    public class LinxNfeIbgeMunicipioValidator : AbstractValidator<LinxNfeIbgeMunicipio>
    {
        public LinxNfeIbgeMunicipioValidator()
        {
            RuleFor(x => x.id_nfe_ibge_municipio)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_nfe_ibge_municipio' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.cod_ibge_municipio)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_ibge_municipio));

            RuleFor(x => x.nome)
                .MaximumLength(100)
                .WithMessage("O campo 'nome' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.nome));

            RuleFor(x => x.cod_ibge_uf)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.cod_ibge_uf));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));

            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));
        }
    }
}
