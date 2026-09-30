using System;
using System.Collections.Generic;
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
                Products = items,
                Categories = _categories.GetActive(),
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
                Images = p.Images,
                Variants = p.Variants,
                RelatedProducts = _products.GetRelated(productId, 4)
            };
        }

        public IList<Category> GetCategories()
        {
            return _categories.GetActive();
        }
    }
}
