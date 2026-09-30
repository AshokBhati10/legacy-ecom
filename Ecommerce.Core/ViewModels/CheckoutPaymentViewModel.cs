using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.ViewModels
{
    /// <summary>
    /// Demo payment step. No payment gateway is integrated (per spec);
    /// card details are validated client-side and never persisted.
    /// </summary>
    public class CheckoutPaymentViewModel
    {
        [Required]
        public string PaymentMethod { get; set; }  // "Card" or "CashOnDelivery"

        [RequiredIfCard, StringLength(100)]
        public string CardholderName { get; set; }

        [RequiredIfCard, CreditCard, StringLength(19)]
        public string CardNumber { get; set; }

        [RequiredIfCard, Range(1, 12)]
        public int? ExpiryMonth { get; set; }

        [RequiredIfCard, Range(2026, 2040)]
        public int? ExpiryYear { get; set; }

        [RequiredIfCard, StringLength(4, MinimumLength = 3)]
        public string Cvv { get; set; }
    }

    /// <summary>
    /// Applies Required validation only when PaymentMethod == "Card".
    /// </summary>
    public class RequiredIfCardAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var vm = validationContext.ObjectInstance as CheckoutPaymentViewModel;
            if (vm != null && vm.PaymentMethod == "Card")
            {
                if (value == null || (value is string && string.IsNullOrWhiteSpace((string)value)))
                    return new ValidationResult(ErrorMessage ?? (validationContext.DisplayName + " is required for card payment."));
            }
            return ValidationResult.Success;
        }
    }
}
