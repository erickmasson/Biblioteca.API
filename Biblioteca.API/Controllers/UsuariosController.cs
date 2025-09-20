using Biblioteca.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Biblioteca.API.Services;
using static Biblioteca.API.Dtos.UsuarioDto;

namespace Biblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsuariosController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(UsuarioRegistroDto usuarioDto)
        {
            var resultado = await _authService.RegistrarAsync(usuarioDto);

            if (!resultado.Sucesso)
            {
                return BadRequest(resultado.Mensagem);
            }
            return Ok(new { message = resultado.Mensagem });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UsuarioLoginDto usuarioDto)
        {
            var resultado = await _authService.LoginAsync(usuarioDto);

            if (!resultado.Sucesso)
            {
                return Unauthorized(resultado.TokenOuMensagem);
            }

            return Ok(new {resultado.TokenOuMensagem});
        }
    }
}