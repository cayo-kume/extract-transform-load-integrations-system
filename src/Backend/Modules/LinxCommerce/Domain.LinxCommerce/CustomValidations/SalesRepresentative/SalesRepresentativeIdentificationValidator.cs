using Domain.LinxCommerce.Entities.SalesRepresentative;
using FluentValidation;

namespace Domain.LinxCommerce.CustomValidations.SalesRepresentative
{
    public class SalesRepresentativeIdentificationValidator : AbstractValidator<SalesRepresentativeIdentification>
    {
        public SalesRepresentativeIdentificationValidator()
        {
            RuleFor(x => x.Type)
                .MaximumLength(1)
                .WithMessage(x => $"Property: Type | Value: {x.Type}, Tamanho do texto: {x.Type.Length} excede ao permitido: 1");

            RuleFor(x => x.DocumentNumber)
                .MaximumLength(14)
                .WithMessage(x => $"Property: DocumentNumber | Value: {x.DocumentNumber}, Tamanho do texto: {x.DocumentNumber.Length} excede ao permitido: 14");
        }
    }
}
