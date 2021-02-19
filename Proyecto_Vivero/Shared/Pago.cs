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
        [Required]
        public int EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Empleado Empleado { get; set; }
        [Required]
        public decimal Importe { get; set; }
        #endregion
    }
}