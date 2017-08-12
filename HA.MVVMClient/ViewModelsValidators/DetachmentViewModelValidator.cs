using FluentValidation;
using HA.MVVMClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HA.MVVMClient.ViewModelsValidators
{
    public class DetachmentViewModelValidator : AbstractValidator<DetachmentViewModel>
    {
        public DetachmentViewModelValidator()
        {
            RuleFor(c => c.Key).
                NotEmpty().WithMessage("Pole nesmí byt prázdné!").
                Length(1, 50).WithMessage("Text v poli musí byt délky 1 - 50 znaku!");
        }
    }
}
