namespace BibliotecaAPI.DTOs
{
    public class AutorCreacionDTOConFoto : AutorCreateDTO
    {
        public IFormFile? Foto { get; set; } 
    }
}
