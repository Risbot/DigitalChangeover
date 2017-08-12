using FluentValidation;
using HA.MVVMClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.ViewModelsValidators
{
    public class CreateChangeoverViewModelValidator: AbstractValidator<CreateChangeoverViewModel>
    {
        public CreateChangeoverViewModelValidator()
        {
            RuleFor(c => c.SelectedVehicle).NotNull().WithMessage("Musí byt vybrán záznam!");
            RuleFor(c => c.SelectedWorkType).NotNull().WithMessage("Musí byt vybrán záznam!");
            RuleFor(c => c.SelectedTopFaultWork).NotNull().WithMessage("Musí byt vybrán záznam, nebo sepsan!").When(c => string.IsNullOrEmpty(c.FaultDescription) == true);
            RuleFor(c => c.FaultDescription).NotEmpty().WithMessage("Musí byt sepsan záznam, nebo vybrán!").When(c => c.SelectedTopFaultWork == null);
        }
    }
}
