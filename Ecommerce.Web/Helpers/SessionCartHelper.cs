using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ecommerce.Core.Entities;

namespace Ecommerce.Web.Helpers
{
    /// <summary>
    /// Session cart owned by the Web layer. Storage is HttpContext.Session
    /// (InProc, 25 minute timeout per Web.config). Services receive the plain
    /// item list and never touch HttpContext.
    /// </summary>
    public class SessionCartHelper
    {
        private const string SessionKey = "Cart";
        private readonly HttpContextBase _context;

        public SessionCartHelper(HttpContextBase context)
        {
            _context = context;
        }

        public IList<CartItem> GetItems()
        {
            var items = _context.Session[SessionKey] as IList<CartItem>;
            if (items == null)
            {
                items = new List<CartItem>();
                _context.Session[SessionKey] = items;
            }
            return items;
        }

        public void SaveItems(IEnumerable<CartItem> items)
        {
            _context.Session[SessionKey] = (items ?? Enumerable.Empty<CartItem>()).ToList();
        }

        public void Clear()
        {
            _context.Session.Remove(SessionKey);
        }
    }
}
