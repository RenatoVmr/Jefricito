using System.ComponentModel.DataAnnotations;

namespace software.Models
{
    public class Venta
    {
        public required string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Código")]
        public required string Codigo { get; set; }

        [Required]
        [Display(Name = "Producto")]
        public required string Producto { get; set; }

        [Required]
        [Display(Name = "Categoría")]
        public required string Categoria { get; set; }

        [Required]
        [Display(Name = "Cantidad")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required]
        [Display(Name = "Precio Unitario")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioUnitario { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Total => Cantidad * PrecioUnitario;

        [Required]
        [Display(Name = "Cliente")]
        [MinLength(3, ErrorMessage = "El nombre del cliente debe tener al menos 3 caracteres")]
        public required string Cliente { get; set; }

        [Required]
        [Display(Name = "Vendido por")]
        public required string VendidoPor { get; set; }

        // Constructor para garantizar inicialización
        public Venta()
        {
            Id = Guid.NewGuid().ToString();
            Fecha = DateTime.Now;
        }
    }

    public class ResumenVentas
    {
        public int TotalVentas { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal UnidadesVendidas { get; set; }
    }
}