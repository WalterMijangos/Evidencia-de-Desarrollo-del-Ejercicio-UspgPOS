using System.ComponentModel.DataAnnotations.Schema;

namespace UspgPOS.Models
{
    public class Producto
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("marca_id")]
        public long MarcaId { get; set; }

        [Column("clasificacion_id")]
        public long ClasificacionId { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        public Marca? Marca { get; set; } // Relacion de uno a uno

        public Clasificacion? Clasificacion { get; set; } // Relacion de uno a uno

    }
}
