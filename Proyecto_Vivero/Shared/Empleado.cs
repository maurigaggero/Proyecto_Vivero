using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Vivero.Shared
{
    public class Empleado
    {
        #region ATRIBUTOS/PROPIEDADES
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Required]
        public string NombreyApellido { get; set; }
        [Required, EnumDataType(typeof(Sexos))]
        public Sexos Sexo { get; set; }
        [Required]
        public string Dirección { get; set; }
        [Required]
        public string Teléfono { get; set; }

        public enum Sexos
        {
            Masculino = 1,
            Femenino = 2,
            Indefinido = 3
        }
        #endregion
    }
}
