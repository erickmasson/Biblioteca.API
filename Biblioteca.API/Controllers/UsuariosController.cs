using Biblioteca.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Biblioteca.API.Dtos.UsuarioDto;

namespace Biblioteca.API.Controllers
{
    public class UsuariosController : ControllerBase
    {
        private readonly BibliotecaContext _context;
        private readonly IConfiguration _configuration;

        public UsuariosController(BibliotecaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UsuarioLoginDto usuarioDto) 
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u=>u.Email == usuarioDto.Email);
            if(usuario == null)
            {
                return Unauthorized("E-mail ou senha inválidos");
            }

            if(!BCrypt.Net.BCrypt.Verify(usuarioDto.Senha, usuario.SenhaHash))
            {
                return Unauthorized("E-mail ou senha inválidos");
            }

            var token = GerarTokenJwt(usuario);

            return Ok(new {token = token});
        }

        private string GerarTokenJwt(Usuario usuario)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, usuario.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
