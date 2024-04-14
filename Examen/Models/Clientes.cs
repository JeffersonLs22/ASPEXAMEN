using System;

namespace Examen.Models
{
    public class Clientes
    {
        public int Codigo { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public int TipoDocumento { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
