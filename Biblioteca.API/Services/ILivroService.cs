using Biblioteca.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Services
{
    /// <summary>
    /// Define o contrato para os serviços de gerenciamento de livros.
    /// </summary>
    public interface ILivroService
    {
        /// <summary>
        /// Obtém uma lista paginada e filtrada de todos os livros.
        /// </summary>
        /// <param name="pageNumber">O número da página a ser retornada.</param>
        /// <param name="pageSize">O número de itens por página.</param>
        /// <param name="searchQuery">Termo de busca para título ou autor.</param>
        /// <param name="genero">Gênero para filtrar os livros.</param>
        /// <returns>Um resultado paginado contendo a lista de livros.</returns>
        Task<PagedResult<Livro>> GetTodosLivrosAsync(int pageNumber, int pageSize, string? searchQuery, string? genero);

        /// <summary>
        /// Obtém um livro específico pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do livro.</param>
        /// <returns>O livro encontrado ou nulo se não existir.</returns>
        Task<Livro?> GetLivroPorIdAsync(int id);

        /// <summary>
        /// Cria um novo livro e faz o upload do seu arquivo PDF.
        /// </summary>
        /// <param name="livroDto">Os dados do livro a ser criado.</param>
        /// <returns>A entidade do livro recém-criado.</returns>
        Task<Livro> CreateLivroAsync(CreateLivroDto livroDto);

        /// <summary>
        /// Atualiza os dados de um livro existente.
        /// </summary>
        /// <param name="id">O ID do livro a ser atualizado.</param>
        /// <param name="livroDto">Os novos dados para o livro.</param>
        /// <returns>Verdadeiro se a atualização for bem-sucedida, falso caso contrário.</returns>
        Task<bool> UpdateLivroAsync(int id, UpdateLivroDto livroDto);

        /// <summary>
        /// Deleta um livro e seu arquivo PDF associado.
        /// </summary>
        /// <param name="id">O ID do livro a ser deletado.</param>
        /// <returns>Verdadeiro se a deleção for bem-sucedida, falso caso contrário.</returns>
        Task<bool> DeleteLivroAsync(int id);

        /// <summary>
        /// Obtém o stream do arquivo PDF de um livro para download.
        /// </summary>
        /// <param name="id">O ID do livro.</param>
        /// <returns>Uma tupla contendo o stream, o tipo de conteúdo e o nome do arquivo, ou nulo se não encontrado.</returns>
        Task<(Stream stream, string contentType, string fileName)?> GetPdfStreamAsync(int id);
    }
}
