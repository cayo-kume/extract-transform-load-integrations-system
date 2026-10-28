using Domain.Totvs.Dtos.Writes.People.Customers;
using FluentValidation;

namespace Application.Totvs.Validations.Writes.People.Customers
{
    /// <summary>
    /// conta bancária da pessoa física (BankAccountInDto): conta, dígito e chave PIX vão mascarados para totvs.TotvsRequests (T6): tamanhos do swagger; nulo não muda, "" e 0 zeram
    /// </summary>
    public class BankAccountInValidator : AbstractValidator<BankAccountIn>
    {
        public BankAccountInValidator()
        {
            RuleFor(x => x.holderName)
                .MaximumLength(40)
                .WithMessage("O campo 'holderName' aceita até 40 caracteres.");

            RuleFor(x => x.additionalData)
                .MaximumLength(80)
                .WithMessage("O campo 'additionalData' aceita até 80 caracteres.");

            RuleFor(x => x.bankNumber)
                .InclusiveBetween(0, 99_999)
                .WithMessage("O campo 'bankNumber' deve ter até 5 dígitos (0 zera).")
                .When(x => x.bankNumber is not null);

            RuleFor(x => x.branchNumber)
                .InclusiveBetween(0, 9_999)
                .WithMessage("O campo 'branchNumber' deve ter até 4 dígitos (0 zera).")
                .When(x => x.branchNumber is not null);

            RuleFor(x => x.accountNumber)
                .MaximumLength(20)
                .WithMessage("O campo 'accountNumber' aceita até 20 caracteres.");

            RuleFor(x => x.personType)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.Person))
                .WithMessage($"O campo 'personType' só aceita {CustomerRules.Names(CustomerEnumNames.Person)}.");

            RuleFor(x => x.accountDigit)
                .MaximumLength(2)
                .WithMessage("O campo 'accountDigit' aceita até 2 caracteres.");

            RuleFor(x => x.accountType)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.BankAccount))
                .WithMessage($"O campo 'accountType' só aceita {CustomerRules.Names(CustomerEnumNames.BankAccount)}.");

            RuleFor(x => x.purposeType)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.BankAccountPurpose))
                .WithMessage($"O campo 'purposeType' só aceita {CustomerRules.Names(CustomerEnumNames.BankAccountPurpose)}.");

            RuleFor(x => x.compensationType)
                .InclusiveBetween(0, 999)
                .WithMessage("O campo 'compensationType' deve ter até 3 dígitos (0 zera).")
                .When(x => x.compensationType is not null);

            RuleFor(x => x.checkType)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.BankAccountCheck))
                .WithMessage($"O campo 'checkType' só aceita {CustomerRules.Names(CustomerEnumNames.BankAccountCheck)}.");

            RuleFor(x => x.pixKeyType)
                .Must(v => CustomerRules.IsEnum(v, CustomerEnumNames.PixKey))
                .WithMessage($"O campo 'pixKeyType' só aceita {CustomerRules.Names(CustomerEnumNames.PixKey)}.");

            RuleFor(x => x.pixKey)
                .MaximumLength(100)
                .WithMessage("O campo 'pixKey' aceita até 100 caracteres.");
        }
    }
}
