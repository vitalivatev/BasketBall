namespace MVC.Intro.Models
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal GrandTotal => Items.Sum(i => i.LineTotal);
    }
}
