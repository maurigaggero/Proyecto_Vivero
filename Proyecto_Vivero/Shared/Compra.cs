using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Vivero.Shared
{
    public class Compra
    {
        #region ATRIBUTOS/PROPIEDADES
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public List<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();
        [Required]
        public string Proveedor { get; set; }
        public string EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public decimal Total { get; set; }
        #endregion
    }

    public class DetalleCompra
    {
        #region ATRIBUTOS/PROPIEDADES
        [Key]
        public int Id { get; set; }
        [Required]
        public int ArticuloId { get; set; }
        [ForeignKey("ArticuloId")]
        public Articulo Articulo { get; set; }
        [Required]
        [Range(1, 99999999, ErrorMessage = "Valor inválido")]
        public int Cantidad { get; set; }
        [Required]
        public decimal Precio_Mayorista { get; set; }
        [Required]
        public decimal Precio_Unitario { get; set; }
        public decimal SubTotal { get; set; }
        [Required]
        public int CompraId { get; set; }
        [ForeignKey("CompraId")]
        public Compra Compra { get; set; }
        #endregion

        #region MÉTODOS
        public decimal CalcularSubTotal(int cantidad, decimal precio)
        {
            decimal subtotal = (precio * cantidad);
            return subtotal;
        }

        public decimal CalcularUnitario(decimal mayorista, int porcentaje)
        {
            decimal unitario = ((mayorista) + (porcentaje * mayorista) / 100);
            return unitario;
        }
        #endregion
    }
}