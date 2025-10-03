using AlmacenWeb.Models;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.ViewModels
{
    public class VentaViewModel
    {
        public ICollection<DetalleVenta> Carrito { get; set; } = new List<DetalleVenta>();
        public decimal Total { get; set; }
    }
}
