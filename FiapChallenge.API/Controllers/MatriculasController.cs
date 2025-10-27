using FiapChallenge.Application.DTOs;
using FiapChallenge.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapChallenge.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class MatriculasController : ControllerBase
    {
        private readonly IMatriculaService _matriculaService;

        public MatriculasController(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        /// <summary>
        /// Matricula um aluno em uma turma
        /// </summary>
        /// <param name="dto">Dados da matrícula</param>
        /// <returns>Matrícula criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(MatriculaResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] MatriculaCreateDto dto)
        {
            try
            {
                var matricula = await _matriculaService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetAlunosByTurma), new { turmaId = matricula.TurmaId }, matricula);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lista todos os alunos matriculados em uma turma
        /// </summary>
        /// <param name="turmaId">ID da turma</param>
        /// <returns>Lista de alunos matriculados</returns>
        [HttpGet("turma/{turmaId}")]
        [ProducesResponseType(typeof(IEnumerable<MatriculaResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAlunosByTurma(int turmaId)
        {
            var matriculas = await _matriculaService.GetAlunosByTurmaAsync(turmaId);
            return Ok(matriculas);
        }

        /// <summary>
        /// Busca uma matrícula específica por ID
        /// </summary>
        /// <param name="id">ID da matrícula</param>
        /// <returns>Matrícula encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MatriculaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var matricula = await _matriculaService.GetByIdAsync(id);

            if (matricula == null)
                return NotFound(new { message = "Matrícula não encontrada" });

            return Ok(matricula);
        }

        /// <summary>
        /// Remove uma matrícula
        /// </summary>
        /// <param name="id">ID da matrícula</param>
        /// <returns>NoContent se removido com sucesso</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _matriculaService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
