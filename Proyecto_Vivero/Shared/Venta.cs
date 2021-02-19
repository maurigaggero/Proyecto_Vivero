using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Vivero.Shared
{
    public class Venta
    {
        #region ATRIBUTOS/PROPIEDADES
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        [Required, EnumDataType(typeof(FormasPago))]
        public FormasPago FormaPago { get; set; }
        public List<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
        [Range(1, 99999999, ErrorMessage = "Seleccione cliente")]
        public int? ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }
        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        public decimal Total { get; set; }
        #endregion
    }

    public enum FormasPago
    {
        Efectivo = 1,
        MercadoPago = 2,
        CuentaCorriente = 3
    }

    public class DetalleVenta
    {
        #region ATRIBUTOS/PROPIEDADES
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int ArticuloId { get; set; }
        [ForeignKey("ArticuloId")]
        public Articulo Articulo { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public int Descuento { get; set; }
        public decimal SubTotal { get; set; }
        [Required]
        public int VentaId { get; set; }
        [ForeignKey("VentaId")]
        public Venta Venta { get; set; }
        #endregion

        #region MÉTODOS
        public decimal CalcularSubTotal(int cantidad, decimal precio, decimal descuento)
        {
            decimal subtotal = ((precio * cantidad) + (precio * cantidad) * - descuento / 100);
            return subtotal;
        }
        #endregion
    }
}