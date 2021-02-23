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
    public class Pago
    {
        #region ATRIBUTOS/PROPIEDADES
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        [Required]
        [Range(1, 99999999, ErrorMessage = "Seleccione cliente")]
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        [Range(1, 99999999, ErrorMessage = "Seleccione cliente")]
        public Cliente Cliente { get; set; }
        public string EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [Required, EnumDataType(typeof(FormasPago))]
        public FormasPago FormaPago { get; set; }
        [Required]
        public decimal Importe { get; set; }

        public enum FormasPago
        {
            Efectivo = 1,
            MercadoPago = 2,
            Cheque = 3,
            Transferencia = 4
        }
        #endregion
    }
}