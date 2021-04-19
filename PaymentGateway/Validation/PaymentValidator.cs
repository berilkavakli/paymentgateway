using FluentValidation;
using PaymentGateway.Contract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Validation
{
    public class PaymentValidator :AbstractValidator<PaymentRequest>
    {
        public PaymentValidator()
        {
            RuleFor(m => m.CardInformation).SetValidator(new CardValidator());
            RuleFor(m => m.Amount).Must(p => p > 0).WithMessage("Amount must be greater than 0!");
            RuleFor(m => m.Currency).NotEmpty();
        }
    }
}
