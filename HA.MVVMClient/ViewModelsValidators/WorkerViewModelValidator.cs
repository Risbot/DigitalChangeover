using FluentValidation;
using HA.MVVMClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.ViewModelsValidators
{
    public class WorkerViewModelValidator : AbstractValidator<WorkerViewModel>
    {
        public WorkerViewModelValidator()
        {
            RuleFor(c => c.WorkerTours).Cascade(FluentValidation.CascadeMode.StopOnFirstFailure).
                NotNull().WithMessage("Tento seznam musí obsahovat aspoň jeden záznam!").
                Must(c => c.Count > 0).WithMessage("Tento seznam musí obsahovat aspoň jeden záznam!");
            RuleFor(c => c.FirstName).
                NotEmpty().WithMessage("Pole nesmí byt prázdné!").
                Length(1, 20).WithMessage("Text v poli musí byt délky 1 - 20 znaku!");      
            RuleFor(c => c.LastName).
                NotEmpty().WithMessage("Pole nesmí byt prázdné!").
                Length(1, 30).WithMessage("Text v poli musí byt délky 1 - 30 znaku!");    
            RuleFor(c => c.SapNumber).
                NotEmpty().WithMessage("Pole nesmí byt prázdné!").
                Length(1, 20).WithMessage("Text v poli musí byt délky 1 - 20 znaku!");           
            RuleFor(c => c.ServiceNumber).
                NotEmpty().WithMessage("Pole nesmí byt prázdné!").
                Length(1, 20).WithMessage("Text v poli musí byt délky 1 - 20 znaku!");         
            RuleFor(c => c.PersonalEmail).
                Length(0, 50).WithMessage("Pole muže obsahovat maximálně 50 znaku!").
                EmailAddress().WithMessage("Email není zadán ve správném formátu!").
                When(c=>string.IsNullOrWhiteSpace(c.PersonalEmail) != true);
            RuleFor(c => c.ServiceEmail).
                Length(0, 50).WithMessage("Pole muže obsahovat maximálně 50 znaku!").
                EmailAddress().WithMessage("Email není zadán ve správném formátu!").
                When(c=>string.IsNullOrWhiteSpace(c.ServiceEmail) != true);
            RuleFor(c => c.PersonalPhone).
                Length(0, 16).WithMessage("Pole musí byt délky 16 znaku!").
                Matches(@"^\+\d{3} \d{3} \d{3} \d{3}$").WithMessage("Telefonní číslo musí byt ve tvaru +XXX XXX XXX XXX!").
                When(c=>string.IsNullOrWhiteSpace(c.PersonalPhone) != true);
            RuleFor(c => c.ServicePhone).
                Length(0, 16).WithMessage("Pole musí byt délky 16 znaku!").
                Matches(@"^\+\d{3} \d{3} \d{3} \d{3}$").WithMessage("Telefonní číslo musí byt ve tvaru +XXX XXX XXX XXX!").
                When(c => string.IsNullOrWhiteSpace(c.ServicePhone) != true);
            
        }
    }
}
