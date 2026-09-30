namespace Ecommerce.Core.ViewModels
{
    /// <summary>
    /// Small view model for category navigation. Views bind to this, never
    /// to the Category entity.
    /// </summary>
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public bool HasChildren { get; set; }
    }
}
