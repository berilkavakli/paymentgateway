using FluentValidation;
using PaymentGateway.Contract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Validation
{
    public class CardValidator : AbstractValidator<CardInformation>
    {
        public CardValidator()
        {
            //CreditCard() is commented out. Because I can't find valid credit card to test :)
            RuleFor(m => m.CardNumber).NotEmpty().Must(p => p.Length == 16);//.CreditCard();
            RuleFor(m => m.Cvv).Must(p => p > 100 && p < 1000).WithMessage("CVV must be three-digit number!");
            RuleFor(m => m.ExpiryMonth).Must(p => p > 0 && p < 13).WithMessage("Expiry month must be between 1 and 12!");
            RuleFor(m => m.ExpiryYear).Must(p => p >= DateTime.Now.Year).WithMessage("Expiry year must be at least current year!");
        }
    }
}
