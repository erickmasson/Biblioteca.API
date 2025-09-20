using System.ComponentModel.DataAnnotations;

namespace Biblioteca.API.Dtos
{
    public class UsuarioDto
    {
        public class UsuarioRegistroDto
        {
            [Required]
            public string Nome { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }
            
            [Required]
            public string Senha { get; set; }
        }

        public class UsuarioLoginDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Senha { get; set; }
        }
    }
}
