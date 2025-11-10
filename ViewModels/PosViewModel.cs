using AlmacenWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.ViewModels
{
    public class PosViewModel
    {
        
        public List<CarritoItemViewModel> Carrito { get; set; } = new List<CarritoItemViewModel>();

        
        public decimal TotalCarrito { get; set; }

        
        public int ProductoSeleccionadoId { get; set; }
        public SelectList ProductosDisponibles { get; set; }

        [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100")]
        public int Cantidad { get; set; } = 1;

        
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        [Display(Name = "Cliente")]
        public int ClienteSeleccionadoId { get; set; }
        public SelectList ClientesDisponibles { get; set; }

       
        [Required(ErrorMessage = "Debe seleccionar un método de pago.")]
        [Display(Name = "Método de Pago")]
        public MetodoPago MetodoPagoSeleccionado { get; set; }
    }
}
