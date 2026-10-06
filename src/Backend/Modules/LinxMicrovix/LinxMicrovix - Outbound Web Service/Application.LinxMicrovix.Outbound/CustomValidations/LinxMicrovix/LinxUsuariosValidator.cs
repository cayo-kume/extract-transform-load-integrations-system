using Application.Core.Validations.Extensions;
using Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix;
using FluentValidation;

namespace Application.LinxMicrovix.Outbound.WebService.CustomValidations.LinxMicrovix
{
    public class LinxUsuariosValidator : AbstractValidator<LinxUsuarios>
    {
        public LinxUsuariosValidator()
        {
            RuleFor(x => x.usuario_id).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.usuario_id));
            RuleFor(x => x.usuario_login).MaximumLength(30).WithMessage("O campo 'usuario_login' deve ter no máximo 30 caracteres.").When(x => !string.IsNullOrEmpty(x.usuario_login));
            RuleFor(x => x.usuario_nome).MaximumLength(50).WithMessage("O campo 'usuario_nome' deve ter no máximo 50 caracteres.").When(x => !string.IsNullOrEmpty(x.usuario_nome));
            RuleFor(x => x.usuario_email).MaximumLength(60).WithMessage("O campo 'usuario_email' deve ter no máximo 60 caracteres.").When(x => !string.IsNullOrEmpty(x.usuario_email));
            RuleFor(x => x.usuario_grupo_id).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.usuario_grupo_id));
            RuleFor(x => x.grupo_usuarios).MaximumLength(1).WithMessage("O campo 'grupo_usuarios' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.grupo_usuarios));
            RuleFor(x => x.usuario_supervisor).MaximumLength(1).WithMessage("O campo 'usuario_supervisor' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.usuario_supervisor));
            RuleFor(x => x.usuario_doc).MaximumLength(14).WithMessage("O campo 'usuario_doc' deve ter no máximo 14 caracteres.").When(x => !string.IsNullOrEmpty(x.usuario_doc));
            RuleFor(x => x.vendedor).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.vendedor));
            RuleFor(x => x.data_criacao).MustBeValidDateTime().When(x => !string.IsNullOrEmpty(x.data_criacao));
            RuleFor(x => x.desativado).MaximumLength(1).WithMessage("O campo 'desativado' deve ter no máximo 1 caractere.").When(x => !string.IsNullOrEmpty(x.desativado));
            RuleFor(x => x.portal).MustBeValidInt32().When(x => !string.IsNullOrEmpty(x.portal));
            RuleFor(x => x.timestamp).MustBeValidInt64().When(x => !string.IsNullOrEmpty(x.timestamp));
        }
    }
}