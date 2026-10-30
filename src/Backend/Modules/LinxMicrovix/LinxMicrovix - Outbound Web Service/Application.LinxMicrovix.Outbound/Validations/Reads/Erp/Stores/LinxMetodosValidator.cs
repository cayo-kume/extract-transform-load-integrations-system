using Domain.LinxMicrovix.Outbound.Dtos.Reads.Erp.Stores;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.Validations.Reads.Erp.Stores
{
    /// <summary>
    /// só registra mensagens no log; os tamanhos são os da v274 e das colunas do Script004_Create_LinxMicrovix_Stores_Tables
    /// </summary>
    public class LinxMetodosValidator : AbstractValidator<LinxMetodos>
    {
        public LinxMetodosValidator()
        {
            RuleFor(x => x.retorno)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'retorno' é obrigatório.")
                .MaximumLength(2200)
                .WithMessage("O campo 'retorno' deve ter no máximo 2200 caracteres.");
        }
    }
}
