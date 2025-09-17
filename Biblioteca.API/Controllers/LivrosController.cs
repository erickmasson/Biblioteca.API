using Biblioteca.API.Data;
using Biblioteca.API.Dtos;
using Biblioteca.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Biblioteca.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly ILivroService _livroService;
        public LivrosController(ILivroService livroService)
        {
            _livroService = livroService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLivros()
        {
            var livros = await _livroService.GetTodosLivrosAsync();
            return Ok(livros);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetLivro(int id)
        {
            var livro = await _livroService.GetLivroPorIdAsync(id);

            if (livro == null)
            {
                return NotFound();
            }

            return Ok(livro);
        }

        [HttpPost]
        [Authorize]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 100 * 1024 * 1024)]
        public async Task<IActionResult> Postlivro([FromForm] CreateLivroDto livroDto) {
            try
            {
                var novoLivro = await _livroService.CreateLivroAsync(livroDto);
                return CreatedAtAction(nameof(GetLivro), new { id = novoLivro.Id }, novoLivro);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{Id}")]
        [Authorize]
        public async Task<IActionResult> UpdateLivro(int id, UpdateLivroDto livroDto)
        {
            var sucesso = await _livroService.UpdateLivroAsync(id, livroDto);
            if (!sucesso)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLivro(int id)
        {
            var sucesso = await _livroService.DeleteLivroAsync(id);
            if (!sucesso)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var resultado = await _livroService.GetPdfStreamAsync(id);
            if(resultado == null)
            {
                return NotFound("Livro ou arquivo não encontrado");
            }

            return File(resultado.Value.stream, resultado.Value.contentType, resultado.Value.fileName);
        }
    }
}
