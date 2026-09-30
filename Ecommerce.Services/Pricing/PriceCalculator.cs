using System;
using System.Collections.Generic;
using System.Linq;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Services.Pricing
{
    /// <summary>
    /// Pricing calculation logic. Pure functions; no I/O, no HttpContext.
    /// </summary>
    /// <remarks>
    /// Demo pricing rules: the class specification does not prescribe shipping
    /// rates, the free-shipping threshold, or the tax rate. These constants
    /// stand in for values that would normally come from configuration or the
    /// database; they are centralized here so they can be replaced in one place.
    /// </remarks>
    public static class PriceCalculator
    {
        public const string StandardShippingCode = "Standard";
        public const string ExpressShippingCode = "Express";

        private const decimal StandardShippingFlatRate = 4.99m;
        private const decimal ExpressShippingFlatRate = 12.99m;
        private const decimal FreeShippingThreshold = 50.00m;
        private const decimal TaxRate = 0.08m;

        public static decimal CalculateLineTotal(decimal unitPrice, int quantity)
        {
            if (quantity < 0) quantity = 0;
            return Math.Round(unitPrice * quantity, 2);
        }

        public static decimal CalculateSubTotal(IEnumerable<CartLineViewModel> lines)
        {
            if (lines == null) return 0m;
            return Math.Round(lines.Sum(l => l.LineTotal), 2);
        }

        public static decimal CalculateShippingCost(decimal subTotal, string shippingMethod)
        {
            if (string.Equals(shippingMethod, ExpressShippingCode, StringComparison.OrdinalIgnoreCase))
                return ExpressShippingFlatRate;

            // Standard: free once the threshold is reached.
            return subTotal >= FreeShippingThreshold ? 0m : StandardShippingFlatRate;
        }

        public static decimal CalculateTax(decimal subTotal)
        {
            if (subTotal < 0) subTotal = 0;
            return Math.Round(subTotal * TaxRate, 2);
        }

        public static decimal CalculateTotal(decimal subTotal, decimal shippingCost, decimal taxAmount)
        {
            return Math.Round(subTotal + shippingCost + taxAmount, 2);
        }

        public static IList<ShippingOptionViewModel> GetShippingOptions(decimal subTotal)
        {
            return new List<ShippingOptionViewModel>
            {
                new ShippingOptionViewModel
                {
                    Code = StandardShippingCode,
                    Name = "Standard",
                    Description = "3-5 business days" + (subTotal >= FreeShippingThreshold ? " — FREE" : ""),
                    Cost = CalculateShippingCost(subTotal, StandardShippingCode)
                },
                new ShippingOptionViewModel
                {
                    Code = ExpressShippingCode,
                    Name = "Express",
                    Description = "1-2 business days",
                    Cost = CalculateShippingCost(subTotal, ExpressShippingCode)
                }
            };
        }

        public static string NormalizeShippingMethod(string shippingMethod)
        {
            if (string.Equals(shippingMethod, ExpressShippingCode, StringComparison.OrdinalIgnoreCase))
                return ExpressShippingCode;
            return StandardShippingCode;
        }
    }
}
