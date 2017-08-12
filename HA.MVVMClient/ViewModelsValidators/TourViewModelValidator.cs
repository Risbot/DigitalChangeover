using FluentValidation;
using HA.MVVMClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.ViewModelsValidators
{
    public class TourViewModelValidator : AbstractValidator<TourViewModel>
    {
        public TourViewModelValidator()
        {
            RuleFor(c => c.StartTime).NotNull().WithMessage("Pole nesmí byt prázdné!").Matches(@"^([01]{0,1}\d|2[0-3]):[0-5]\d$").WithMessage("Čas je v nesprávném formátu. Použijte 24 hodinový formát!");
            RuleFor(c => c.EndTime).NotNull().WithMessage("Pole nesmí byt prázdné!").Matches(@"^([01]{0,1}\d|2[0-3]):[0-5]\d$").WithMessage("Čas je v nesprávném formátu. Použijte 24 hodinový formát!");
        }
    }
}
