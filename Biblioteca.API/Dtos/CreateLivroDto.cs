using System.ComponentModel.DataAnnotations;

namespace Biblioteca.API.Dtos
{
    public class CreateLivroDto
    {
        [Required]
        public string Titulo { get; set; }
        [Required]
        public string Autor { get; set; }
        public string Genero { get; set; }
        public int AnoPublicacao { get; set; }
        [Required]
        public IFormFile ArquivoPdf { get; set; }
    }
}
