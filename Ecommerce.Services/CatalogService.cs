using System;
using System.Collections.Generic;
using System.Linq;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;
using Ecommerce.Core.Interfaces.Services;
using Ecommerce.Core.ViewModels;

namespace Ecommerce.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;

        public CatalogService(IProductRepository products, ICategoryRepository categories)
        {
            _products = products;
            _categories = categories;
        }

        public ProductListViewModel GetListing(int? categoryId, string q, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 12;

            int totalCount;
            var items = _products.GetListing(categoryId, q, page, pageSize, out totalCount);

            var vm = new ProductListViewModel
            {
                Products = items.Select(ToCardViewModel).ToList(),
                Categories = _categories.GetActive().Select(ToCategoryViewModel).ToList(),
                SelectedCategoryId = categoryId,
                SearchQuery = q,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
            vm.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return vm;
        }

        public ProductDetailViewModel GetDetail(int productId)
        {
            var p = _products.GetById(productId);
            if (p == null) return null;

            return new ProductDetailViewModel
            {
                Product = p,
                Images = p.Images != null ? p.Images.ToList() : new List<ProductImage>(),
                Variants = p.Variants != null ? p.Variants.ToList() : new List<ProductVariant>(),
                RelatedProducts = _products.GetRelated(productId, 4).Select(ToCardViewModel).ToList()
            };
        }

        public IList<CategoryViewModel> GetCategories()
        {
            return _categories.GetActive().Select(ToCategoryViewModel).ToList();
        }

        public IList<CategoryViewModel> GetCategoryTree(int? parentId)
        {
            return _categories.GetChildren(parentId).Select(ToCategoryViewModel).ToList();
        }

        private static CategoryViewModel ToCategoryViewModel(Category c)
        {
            return new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                Description = c.Description,
                HasChildren = c.ChildCategories != null && c.ChildCategories.Any(x => x.IsActive)
            };
        }

        private static ProductCardViewModel ToCardViewModel(Product p)
        {
            return new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                SalePrice = p.SalePrice,
                ThumbnailUrl = p.ThumbnailUrl
            };
        }
    }
}
