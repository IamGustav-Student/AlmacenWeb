using AlmacenWeb.Models;

namespace AlmacenWeb.ViewModels
{
    public class VentaViewModel
    {
        public ICollection<DetalleVenta> Carrito { get; set; } = new List<DetalleVenta>();
        public decimal Total { get; set; }
    }
}
