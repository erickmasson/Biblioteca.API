using Biblioteca.API.Data;
using Biblioteca.API.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Biblioteca.API.Dtos.UsuarioDto;
using Microsoft.Extensions.Logging;

namespace Biblioteca.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly BibliotecaContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(BibliotecaContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(bool Sucesso, string Mensagem)> RegistrarAsync(UsuarioRegistroDto usuarioDto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
            {
                return (false, "Este e-mail já está em uso.");
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

            return (true, "Usuário registrado com sucesso!");
        }

        public async Task<(bool Sucesso, string TokenOuMensagem)> LoginAsync(UsuarioLoginDto usuarioDto)
        {
            _logger.LogInformation("Tentativa de login para o e-mail: {Email}", usuarioDto.Email);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuarioDto.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(usuarioDto.Senha, usuario.SenhaHash))
            {
                return (false, "E-mail ou senha inválidos.");
            }

            _logger.LogInformation("Usuário {UsuarioId} autenticado com sucesso.", usuario.Id);

            var token = GerarTokenJwt(usuario);
            return (true, token);
        }
        private string GerarTokenJwt(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, usuario.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

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