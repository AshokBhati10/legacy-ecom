using System;
using System.Collections.Generic;
using System.Linq;
using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;
using Ecommerce.Services.Pricing;

namespace Ecommerce.Services
{
    public class CartService : ICartService
    {
        private readonly IProductRepository _products;
        private readonly ICartRepository _carts;
        private readonly IUnitOfWork _unitOfWork;

        private const int MaxQuantityPerLine = 99;

        public CartService(IProductRepository products, ICartRepository carts, IUnitOfWork unitOfWork)
        {
            _products = products;
            _carts = carts;
            _unitOfWork = unitOfWork;
        }

        public CartViewModel GetCart(IEnumerable<CartItem> items)
        {
            var vm = new CartViewModel();
            foreach (var item in items ?? Enumerable.Empty<CartItem>())
            {
                var line = BuildLine(item);
                if (line != null) vm.Items.Add(line);
            }
            vm.ItemCount = vm.Items.Sum(x => x.Quantity);
            vm.SubTotal = PriceCalculator.CalculateSubTotal(vm.Items);
            return vm;
        }

        public MiniCartViewModel GetMiniCart(IEnumerable<CartItem> items)
        {
            var cart = GetCart(items);
            return new MiniCartViewModel
            {
                ItemCount = cart.ItemCount,
                SubTotal = cart.SubTotal
            };
        }

        public ServiceResult<IEnumerable<CartItem>> AddItem(IEnumerable<CartItem> items, int productId, int? variantId, int quantity)
        {
            if (quantity < 1) quantity = 1;
            if (quantity > MaxQuantityPerLine) quantity = MaxQuantityPerLine;

            var product = _products.GetById(productId);
            if (product == null)
                return ServiceResult<IEnumerable<CartItem>>.Fail("Product not found.");

            if (variantId.HasValue)
            {
                var variant = product.Variants.FirstOrDefault(v => v.Id == variantId.Value);
                if (variant == null)
                    return ServiceResult<IEnumerable<CartItem>>.Fail("Product variant not found.");
            }

            var list = (items ?? Enumerable.Empty<CartItem>())
                .Select(x => new CartItem { ProductId = x.ProductId, VariantId = x.VariantId, Quantity = x.Quantity })
                .ToList();

            var existing = list.FirstOrDefault(x => x.ProductId == productId && x.VariantId == variantId);
            if (existing != null)
            {
                existing.Quantity = Math.Min(existing.Quantity + quantity, MaxQuantityPerLine);
            }
            else
            {
                list.Add(new CartItem { ProductId = productId, VariantId = variantId, Quantity = quantity });
            }

            return ServiceResult<IEnumerable<CartItem>>.Ok(list, "Added to cart.");
        }

        public ServiceResult<IEnumerable<CartItem>> UpdateItem(IEnumerable<CartItem> items, int productId, int? variantId, int quantity)
        {
            var list = (items ?? Enumerable.Empty<CartItem>())
                .Select(x => new CartItem { ProductId = x.ProductId, VariantId = x.VariantId, Quantity = x.Quantity })
                .ToList();

            var existing = list.FirstOrDefault(x => x.ProductId == productId && x.VariantId == variantId);
            if (existing == null)
                return ServiceResult<IEnumerable<CartItem>>.Fail("Item not found in cart.");

            if (quantity <= 0)
                list.Remove(existing);
            else
                existing.Quantity = Math.Min(quantity, MaxQuantityPerLine);

            return ServiceResult<IEnumerable<CartItem>>.Ok(list, "Cart updated.");
        }

        public ServiceResult<IEnumerable<CartItem>> RemoveItem(IEnumerable<CartItem> items, int productId, int? variantId)
        {
            return UpdateItem(items, productId, variantId, 0);
        }

        public IEnumerable<CartItem> ClearCart()
        {
            return new List<CartItem>();
        }

        public void SaveForUser(string userId, IEnumerable<CartItem> items)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;
            _carts.Save(userId, items ?? Enumerable.Empty<CartItem>());
            _unitOfWork.SaveChanges();
        }

        public IList<CartItem> LoadForUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return new List<CartItem>();
            return _carts.GetByUserId(userId);
        }

        private CartLineViewModel BuildLine(CartItem item)
        {
            var product = _products.GetById(item.ProductId);
            if (product == null) return null;

            var variantName = (string)null;
            var unitPrice = product.EffectivePrice;
            if (item.VariantId.HasValue)
            {
                var variant = product.Variants.FirstOrDefault(v => v.Id == item.VariantId.Value);
                if (variant == null) return null;
                variantName = variant.Name;
                unitPrice += variant.PriceAdjustment;
            }

            return new CartLineViewModel
            {
                ProductId = product.Id,
                VariantId = item.VariantId,
                ProductName = product.Name,
                VariantName = variantName,
                ThumbnailUrl = product.ThumbnailUrl,
                UnitPrice = unitPrice,
                Quantity = item.Quantity,
                LineTotal = PriceCalculator.CalculateLineTotal(unitPrice, item.Quantity)
            };
        }
    }
}
