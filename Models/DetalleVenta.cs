﻿using System.ComponentModel.DataAnnotations.Schema;

namespace UspgPOS.Models
{
    public class DetalleVenta
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("venta_id")]
        public long VentaId { get; set; }

        [Column("producto_id")]
        public long ProductoId { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_unitario")]
        public decimal PrecioUnitario { get; set; }

       

        public Venta? Venta { get; set; } // Relacion de uno a uno

        public Producto? Producto { get; set; } // Relacion de uno a uno
    }
}