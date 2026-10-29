using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Catalog;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Catalog
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceProductsMedias (scripts de tabelas do módulo)
    /// </summary>
    public class ProductMediaValidator : AbstractValidator<ProductMedia>
    {
        public ProductMediaValidator()
        {
            RuleFor(x => x.MediaID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'MediaID' é obrigatório.")
                .MustBeValidInt32()
                .WithMessage("O campo 'MediaID' deve ser um número inteiro.");

            RuleFor(x => x.Type)
                .MaximumLength(50)
                .WithMessage("O campo 'Type' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Type));

            RuleFor(x => x.Index)
                .MaximumLength(50)
                .WithMessage("O campo 'Index' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.Index));

            RuleFor(x => x.IsDeleted)
                .MustBeValidBoolean()
                .WithMessage("O campo 'IsDeleted' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.IsDeleted));

            RuleFor(x => x.CreatedDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'CreatedDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.CreatedDate));

            RuleFor(x => x.ParentMediaID)
                .MustBeValidInt32()
                .WithMessage("O campo 'ParentMediaID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.ParentMediaID));

            RuleFor(x => x.OriginalFileName)
                .MaximumLength(400)
                .WithMessage("O campo 'OriginalFileName' deve ter no máximo 400 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.OriginalFileName));

            RuleFor(x => x.Image!.RelativePath)
                .MaximumLength(1000)
                .WithMessage("O campo 'Image.RelativePath' deve ter no máximo 1000 caracteres e foi cortado.")
                .When(x => x.Image != null && !String.IsNullOrEmpty(x.Image.RelativePath));

            RuleFor(x => x.Image!.AbsolutePath)
                .MaximumLength(1000)
                .WithMessage("O campo 'Image.AbsolutePath' deve ter no máximo 1000 caracteres e foi cortado.")
                .When(x => x.Image != null && !String.IsNullOrEmpty(x.Image.AbsolutePath));

            RuleFor(x => x.Image!.Extension)
                .MaximumLength(20)
                .WithMessage("O campo 'Image.Extension' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => x.Image != null && !String.IsNullOrEmpty(x.Image.Extension));

            RuleFor(x => x.Image!.MaxWidth)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Image.MaxWidth' deve ser um número decimal.")
                .When(x => x.Image != null && !String.IsNullOrEmpty(x.Image.MaxWidth));

            RuleFor(x => x.Image!.MaxHeight)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Image.MaxHeight' deve ser um número decimal.")
                .When(x => x.Image != null && !String.IsNullOrEmpty(x.Image.MaxHeight));

            RuleFor(x => x.Image!.Width)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Image.Width' deve ser um número decimal.")
                .When(x => x.Image != null && !String.IsNullOrEmpty(x.Image.Width));

            RuleFor(x => x.Image!.Height)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Image.Height' deve ser um número decimal.")
                .When(x => x.Image != null && !String.IsNullOrEmpty(x.Image.Height));

            RuleFor(x => x.Image!.MediaSizeType)
                .MustBeValidInt32()
                .WithMessage("O campo 'Image.MediaSizeType' deve ser um número inteiro.")
                .When(x => x.Image != null && !String.IsNullOrEmpty(x.Image.MediaSizeType));

            RuleFor(x => x.Video!.Title)
                .MaximumLength(400)
                .WithMessage("O campo 'Video.Title' deve ter no máximo 400 caracteres e foi cortado.")
                .When(x => x.Video != null && !String.IsNullOrEmpty(x.Video.Title));

            RuleFor(x => x.Video!.Url)
                .MaximumLength(1000)
                .WithMessage("O campo 'Video.Url' deve ter no máximo 1000 caracteres e foi cortado.")
                .When(x => x.Video != null && !String.IsNullOrEmpty(x.Video.Url));
        }
    }
}
