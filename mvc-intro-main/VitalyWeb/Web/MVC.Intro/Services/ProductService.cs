using MVC.Intro.Data;
using MVC.Intro.Models;

namespace MVC.Intro.Services
{
    public class ProductService
    {
        private const string ProductPrefix = "PRD_";
        private readonly ILogger<ProductService> _logger;
        private readonly AppDbContext _context;


        public ProductService(ILogger<ProductService> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }
        public Product GetProductById(Guid id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                throw new ArgumentNullException("Product not found");
            }
            return product;
        }

        public Product AddProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product is null");
            }

            var toAdd = new Product
            {
                Name = product.Name,
                Price = product.Price,
                ImagePath = NormalizeImagePath(product.ImagePath)
            };
            _logger.LogInformation("Adding product: {ProductName} with price {ProductPrice}", toAdd.Name, toAdd.Price);
            toAdd.Id = Guid.NewGuid();
            if (!toAdd.Name.StartsWith(ProductPrefix))
            {
                DecorateProductName(toAdd);
            }
            _context.Products.Add(toAdd);
            _context.SaveChanges();
            return toAdd;
        }
        public void DeleteProduct(Guid id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                throw new ArgumentNullException("Product not found");
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        private void DecorateProductName(Product product)
        {
            product.Name = ProductPrefix + product.Name;
        }

        private static string? NormalizeImagePath(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            var normalized = imagePath.Trim().Replace("\\", "/").TrimStart('/');
            const string imageFolderPrefix = "images/";
            if (normalized.StartsWith(imageFolderPrefix, StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized[imageFolderPrefix.Length..];
            }

            return normalized;
        }

    }
}
