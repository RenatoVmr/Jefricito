using System.ComponentModel.DataAnnotations;

namespace software.Models
{
    public class Venta
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required]
        [Display(Name = "Producto")]
        public string Producto { get; set; }

        [Required]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; }

        [Required]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Display(Name = "Precio Unitario")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal PrecioUnitario { get; set; }

        [Display(Name = "Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Total => Cantidad * PrecioUnitario;

        [Required]
        [Display(Name = "Cliente")]
        public string Cliente { get; set; }

        [Required]
        [Display(Name = "Vendido por")]
        public string VendidoPor { get; set; }
    }

    public class ResumenVentas
    {
        public int TotalVentas { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal UnidadesVendidas { get; set; }
    }
}