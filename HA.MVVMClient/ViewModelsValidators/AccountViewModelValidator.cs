using FluentValidation;
using HA.MVVMClient.Infrastructure;
using HA.MVVMClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.ViewModelsValidators
{
    public class AccountViewModelValidator : AbstractValidator<AccountViewModel>
    {
        public AccountViewModelValidator()
        {
            RuleFor(c => c.OldPassword).NotEmpty().WithMessage("Pole nesmí byt prázdné!");
            RuleFor(c => c.NewPassword).NotEmpty().WithMessage("Pole nesmí byt prázdné!").Equal(c => c.ConfirmPassword).WithMessage("Zadané heslo neodpovídá heslu v poli 'Potvrzeni hesla'!");
            RuleFor(C => C.ConfirmPassword).NotEmpty().WithMessage("Pole nesmí byt prázdné!").Equal(c => c.NewPassword).WithMessage("Zadané heslo neodpovídá heslu v poli 'Nové heslo'!");
        }
    }
}
