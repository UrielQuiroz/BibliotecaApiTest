using BibliotecaAPI.Entidades;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs
{
    public class ComentarioCreateDTO
    {
        [Required]
        public required string Cuerpo { get; set; }
    }
}
