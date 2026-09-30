using System.Linq;
using CoreEntities = Ecommerce.Core.Entities;
using DataModels = Ecommerce.Data.Models;

namespace Ecommerce.Data.Mapping
{
    /// <summary>
    /// Translates EDMX-mapped entities (Ecommerce.Data.Models) to the
    /// framework-free Core contracts (Ecommerce.Core.Entities) at the
    /// repository boundary. EF types never leave the Data project.
    /// </summary>
    internal static class CoreMapper
    {
        public static CoreEntities.Product ToCore(this DataModels.Product p)
        {
            if (p == null) return null;
            return new CoreEntities.Product
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                Description = p.Description,
                Price = p.Price,
                SalePrice = p.SalePrice,
                CategoryId = p.CategoryId,
                ThumbnailUrl = p.ThumbnailUrl,
                IsActive = p.IsActive,
                IsFeatured = p.IsFeatured,
                StockQuantity = p.StockQuantity,
                CreatedDate = p.CreatedDate,
                Category = p.Category == null ? null : new CoreEntities.Category
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Slug = p.Category.Slug
                },
                Images = p.Images == null
                    ? new System.Collections.Generic.List<CoreEntities.ProductImage>()
                    : p.Images.OrderBy(i => i.DisplayOrder).Select(ToCore).ToList(),
                Variants = p.Variants == null
                    ? new System.Collections.Generic.List<CoreEntities.ProductVariant>()
                    : p.Variants.Where(v => v.IsActive).Select(ToCore).ToList()
            };
        }

        public static CoreEntities.ProductImage ToCore(this DataModels.ProductImage i)
        {
            if (i == null) return null;
            return new CoreEntities.ProductImage
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Url = i.Url,
                AltText = i.AltText,
                DisplayOrder = i.DisplayOrder,
                IsMain = i.IsMain
            };
        }

        public static CoreEntities.ProductVariant ToCore(this DataModels.ProductVariant v)
        {
            if (v == null) return null;
            return new CoreEntities.ProductVariant
            {
                Id = v.Id,
                ProductId = v.ProductId,
                Name = v.Name,
                Sku = v.Sku,
                PriceAdjustment = v.PriceAdjustment,
                StockQuantity = v.StockQuantity,
                IsActive = v.IsActive
            };
        }

        public static CoreEntities.Category ToCore(this DataModels.Category c)
        {
            if (c == null) return null;
            return new CoreEntities.Category
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive,
                ChildCategories = c.ChildCategories == null
                    ? new System.Collections.Generic.List<CoreEntities.Category>()
                    : c.ChildCategories.Where(x => x.IsActive).OrderBy(x => x.DisplayOrder).Select(ToCore).ToList()
            };
        }

        public static CoreEntities.Order ToCore(this DataModels.Order o)
        {
            if (o == null) return null;
            return new CoreEntities.Order
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerId = o.CustomerId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                SubTotal = o.SubTotal,
                ShippingCost = o.ShippingCost,
                TaxAmount = o.TaxAmount,
                Total = o.Total,
                ShippingMethod = o.ShippingMethod,
                PaymentMethod = o.PaymentMethod,
                ShipFirstName = o.ShipFirstName,
                ShipLastName = o.ShipLastName,
                ShipEmail = o.ShipEmail,
                ShipPhone = o.ShipPhone,
                ShipStreet = o.ShipStreet,
                ShipCity = o.ShipCity,
                ShipState = o.ShipState,
                ShipPostalCode = o.ShipPostalCode,
                ShipCountry = o.ShipCountry,
                OrderLines = o.OrderLines == null
                    ? new System.Collections.Generic.List<CoreEntities.OrderLine>()
                    : o.OrderLines.Select(ToCore).ToList()
            };
        }

        public static CoreEntities.OrderLine ToCore(this DataModels.OrderLine l)
        {
            if (l == null) return null;
            return new CoreEntities.OrderLine
            {
                Id = l.Id,
                OrderId = l.OrderId,
                ProductId = l.ProductId,
                VariantId = l.VariantId,
                ProductName = l.ProductName,
                VariantName = l.VariantName,
                Sku = l.Sku,
                UnitPrice = l.UnitPrice,
                Quantity = l.Quantity,
                LineTotal = l.LineTotal
            };
        }

        public static CoreEntities.Customer ToCore(this DataModels.Customer c)
        {
            if (c == null) return null;
            return new CoreEntities.Customer
            {
                Id = c.Id,
                UserId = c.UserId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                CreatedDate = c.CreatedDate
            };
        }

        public static DataModels.Order ToData(this CoreEntities.Order o)
        {
            var d = new DataModels.Order
            {
                OrderNumber = o.OrderNumber,
                CustomerId = o.CustomerId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                SubTotal = o.SubTotal,
                ShippingCost = o.ShippingCost,
                TaxAmount = o.TaxAmount,
                Total = o.Total,
                ShippingMethod = o.ShippingMethod,
                PaymentMethod = o.PaymentMethod,
                ShipFirstName = o.ShipFirstName,
                ShipLastName = o.ShipLastName,
                ShipEmail = o.ShipEmail,
                ShipPhone = o.ShipPhone,
                ShipStreet = o.ShipStreet,
                ShipCity = o.ShipCity,
                ShipState = o.ShipState,
                ShipPostalCode = o.ShipPostalCode,
                ShipCountry = o.ShipCountry
            };
            foreach (var l in o.OrderLines)
            {
                d.OrderLines.Add(new DataModels.OrderLine
                {
                    ProductId = l.ProductId,
                    VariantId = l.VariantId,
                    ProductName = l.ProductName,
                    VariantName = l.VariantName,
                    Sku = l.Sku,
                    UnitPrice = l.UnitPrice,
                    Quantity = l.Quantity,
                    LineTotal = l.LineTotal
                });
            }
            return d;
        }

        public static DataModels.Customer ToData(this CoreEntities.Customer c)
        {
            return new DataModels.Customer
            {
                UserId = c.UserId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                CreatedDate = c.CreatedDate
            };
        }

        public static DataModels.Address ToData(this CoreEntities.Address a)
        {
            return new DataModels.Address
            {
                CustomerId = a.CustomerId,
                Label = a.Label,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Street = a.Street,
                City = a.City,
                State = a.State,
                PostalCode = a.PostalCode,
                Country = a.Country,
                Phone = a.Phone,
                IsDefault = a.IsDefault
            };
        }
    }
}
