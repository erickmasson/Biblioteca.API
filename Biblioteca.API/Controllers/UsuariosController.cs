using Biblioteca.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Biblioteca.API.Dtos.UsuarioDto;

namespace Biblioteca.API.Controllers
{
    public class UsuariosController : ControllerBase
    {
        private readonly BibliotecaContext _context;

        public UsuariosController(BibliotecaContext context)
        {
            _context = context;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(UsuariosRegistroDto usuarioDto)
        {
            if(await _context.Usuarios.AnyAsync(u=> u.Email == usuarioDto.Email))
            {
                return BadRequest("Este e-mail já está em uso.");
            }

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(usuarioDto.Senha, 12);

            var usuario = new Usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                SenhaHash = senhaHash,
                Role = "Membro"
            };

            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuário registrado com sucesso!" });
        }
    }
}
