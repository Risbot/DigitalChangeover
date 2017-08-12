using FluentValidation;
using HA.MVVMClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.ViewModelsValidators
{
    public class AttendanceViewModelValidator : AbstractValidator<AttendanceViewModel>
    {
        public AttendanceViewModelValidator()
        {
            RuleFor(c => c.SelectedWorkerState).NotNull().WithMessage("Musí byt vybrán záznam!").When(c => c.SelectedWorker != null);
            RuleFor(c => c.SelectedWorkerTour).NotNull().WithMessage("Musí byt vybrán záznam!").When(c => c.SelectedWorker != null);
        }
    }
}
