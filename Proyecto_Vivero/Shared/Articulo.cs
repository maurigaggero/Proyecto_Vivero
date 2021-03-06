using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Vivero.Shared
{
    public class Articulo
    {
        #region ATRIBUTOS/PROPIEDADES
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required, EnumDataType(typeof(Categorias))]
        public Categorias Categoria { get; set; }
        public string? Descripcion { get; set; }
        public string? Url_Imagen { get; set; }
        [Required]
        [Range(1, 99999999, ErrorMessage = "Valor inválido")]
        public decimal PrecioMayorista { get; set; }
        [Required]
        [Range(1, 99999999, ErrorMessage = "Valor inválido")]
        public decimal PrecioUnitario { get; set; }
        public DateTime Ultima_Modificación { get; set; }
        [Required]
        [Range(1, 99999999, ErrorMessage = "Valor inválido")]
        public int StockMinimo { get; set; }
        [Required]
        [Range(1, 99999999, ErrorMessage = "Valor inválido")]
        public int StockMaximo { get; set; }
        [Required]
        [Range(0, 99999999, ErrorMessage = "Valor inválido")]
        public int StockActual { get; set; }

        public enum Categorias
        {
            Planta_Adentro = 1,
            Planta_Afuera = 2,
            Maceta = 3,
            Fertilizante = 4,
            Semilla = 5,
            Tierra = 6,
            Otra = 7
        }
        #endregion

        #region MÉTODOS
        public decimal CalcularUnitario(decimal mayorista, int porcentaje)
        {
            decimal unitario = ((mayorista) + (porcentaje * mayorista) / 100);
            return unitario;
        }
        #endregion
    }
}
