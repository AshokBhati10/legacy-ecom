using System;
using System.Collections.Generic;

namespace Ecommerce.Core.Common
{
    /// <summary>
    /// Paged list wrapper used by catalog listing.
    /// </summary>
    public class PagedResult<T>
    {
        public IList<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedResult()
        {
            Items = new List<T>();
        }

        public int TotalPages
        {
            get
            {
                if (PageSize <= 0) return 0;
                return (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }

        public bool HasPreviousPage
        {
            get { return PageNumber > 1; }
        }

        public bool HasNextPage
        {
            get { return PageNumber < TotalPages; }
        }
    }
}
