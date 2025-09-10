using System.ComponentModel.DataAnnotations;

namespace Biblioteca.API.Models
{
    public class Livro
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        [MaxLength(200, ErrorMessage = "O título deve ter no máximo 200 caracteres")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "O autor é obrigatório")]
        [MaxLength(150, ErrorMessage = "O autor deve ter no máximo 150 caracteres")]
        public string Autor { get; set; }

        [MaxLength(100, ErrorMessage = "O gênero deve ter no máximo 100 caracteres")]
        public string Genero { get; set; }
        public int AnoPublicacao { get; set; }

        [Required]
        public string CaminhoPdf { get; set; }
        public DateTime DataUpload { get; set; } = DateTime.UtcNow;
    }
}
