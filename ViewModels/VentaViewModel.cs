using System.Collections.Generic;

namespace AlmacenWeb.ViewModels
{
    // Este ViewModel actúa como un DTO (Data Transfer Object)
    // para recibir los datos del POS (Punto de Venta) desde el frontend.

    public class VentaViewModel
    {
        // ID del cliente seleccionado
        public int ClId { get; set; }

        // ID del usuario que está registrando la venta (lo tomaremos de la sesión)
        public int UsId { get; set; }

        // Monto total de la venta (calculado en el frontend)
        public decimal Total { get; set; }

        // Lista de productos en el "carrito"
        public List<DetalleViewModel> Detalles { get; set; }
    }

    // Sub-clase que representa cada línea del carrito
    public class DetalleViewModel
    {
        public int PrId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}