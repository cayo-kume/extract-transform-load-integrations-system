using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Sellers;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Sellers
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script031_Create_LinxMicrovix_Sellers_Tables
    /// </summary>
    public class LinxRegistroProfissionalValidator : AbstractValidator<LinxRegistroProfissional>
    {
        public LinxRegistroProfissionalValidator()
        {
            RuleFor(x => x.portal)
                .MustBeValidInt32()
                .When(x => !string.IsNullOrEmpty(x.portal));

            RuleFor(x => x.id_tipo_registro_profissional)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'id_tipo_registro_profissional' é obrigatório.")
                .MustBeValidInt32();

            RuleFor(x => x.descricao_tipo_registro_profissional)
                .MaximumLength(100)
                .WithMessage("O campo 'descricao_tipo_registro_profissional' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.descricao_tipo_registro_profissional));

            RuleFor(x => x.ativo)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.ativo));

            RuleFor(x => x.excluido)
                .MustBeValidMicrovixBoolean()
                .When(x => !string.IsNullOrEmpty(x.excluido));

            RuleFor(x => x.timestamp)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}
