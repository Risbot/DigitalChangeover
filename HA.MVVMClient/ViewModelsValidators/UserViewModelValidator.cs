using FluentValidation;
using HA.MVVMClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.ViewModelsValidators
{
    public class UserViewModelValidator : AbstractValidator<UserViewModel>
    {
        public UserViewModelValidator()
        {
            RuleFor(c => c.SelectedDetachment).
                NotNull().WithMessage("Musí byt vybrán záznam!");
            RuleFor(c => c.UserName).
                NotEmpty().WithMessage("Pole nesmí byt prázdné!").
                Length(1, 30).WithMessage("Text v poli musí byt délky 1 - 30 znaku!");
        }
    }
}
