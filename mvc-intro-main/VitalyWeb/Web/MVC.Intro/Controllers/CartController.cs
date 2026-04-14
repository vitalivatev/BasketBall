using Microsoft.AspNetCore.Mvc;
using MVC.Intro.Models;
using MVC.Intro.Services;
using System.Text.Json;

namespace MVC.Intro.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "cart-items";
        private readonly ProductService _productService;

        public CartController(ProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            var cart = GetCartDictionary();
            var products = _productService.GetAllProducts();

            var model = new CartViewModel
            {
                Items = cart
                    .Join(
                        products,
                        kvp => kvp.Key,
                        p => p.Id,
                        (kvp, product) => new CartItemViewModel
                        {
                            ProductId = product.Id,
                            ProductName = product.Name,
                            UnitPrice = product.Price,
                            Quantity = kvp.Value
                        })
                    .OrderBy(i => i.ProductName)
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(Guid id)
        {
            var cart = GetCartDictionary();
            cart.TryGetValue(id, out var quantity);
            cart[id] = quantity + 1;
            SaveCartDictionary(cart);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(Guid id)
        {
            var cart = GetCartDictionary();
            if (cart.Remove(id))
            {
                SaveCartDictionary(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction(nameof(Index));
        }

        private Dictionary<Guid, int> GetCartDictionary()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrWhiteSpace(cartJson))
            {
                return new Dictionary<Guid, int>();
            }

            var cart = JsonSerializer.Deserialize<Dictionary<Guid, int>>(cartJson);
            return cart ?? new Dictionary<Guid, int>();
        }

        private void SaveCartDictionary(Dictionary<Guid, int> cart)
        {
            var sanitized = cart
                .Where(c => c.Value > 0)
                .ToDictionary(c => c.Key, c => c.Value);

            var cartJson = JsonSerializer.Serialize(sanitized);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }
}
