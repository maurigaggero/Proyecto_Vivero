using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_Vivero.Shared
{
    public class Pedido
    {
        #region ATRIBUTOS/PROPIEDADES
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(1, 99999999, ErrorMessage = "Seleccione cliente")]
        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }
        public DateTime Fecha { get; set; }
        [Required]
        public decimal Seña { get; set; }
        public List<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
        [Required]
        public bool Finalizado { get; set; }
        #endregion
    }

    public class DetallePedido
    {
        #region ATRIBUTOS/PROPIEDADES
        [Key]
        public int Id { get; set; }
        [Required]
        public int ArticuloId { get; set; }
        [ForeignKey("ArticuloId")]
        public Articulo Articulo { get; set; }
        [Required]
        public int Cantidad { get; set; }
        public int PedidoId { get; set; }
        [ForeignKey("PedidoId")]
        public Pedido Pedido { get; set; }
        #endregion
    }
}