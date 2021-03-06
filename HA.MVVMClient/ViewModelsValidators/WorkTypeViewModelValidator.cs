﻿using FluentValidation;
using HA.MVVMClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.ViewModelsValidators
{
    public class WorkTypeViewModelValidator : AbstractValidator<WorkTypeViewModel>
    {
        public WorkTypeViewModelValidator()
        {
            RuleFor(c => c.Key).
                NotEmpty().WithMessage("Pole nesmí byt prázdné!").
                Length(1, 30).WithMessage("Text v poli musí byt délky 1 - 30 znaku!");
        }
    }
}
