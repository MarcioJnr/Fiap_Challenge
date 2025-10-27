using FiapChallenge.Application.DTOs;
using FiapChallenge.Application.Services;
using FiapChallenge.Application.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapChallenge.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class TurmasController : ControllerBase
    {
        private readonly ITurmaService _turmaService;

        public TurmasController(ITurmaService turmaService)
        {
            _turmaService = turmaService;
        }

        /// <summary>
        /// Lista todas as turmas com paginação
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Itens por página (padrão: 10)</param>
        /// <returns>Lista paginada de turmas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (items, totalCount) = await _turmaService.GetAllAsync(pageNumber, pageSize);

            return Ok(new
            {
                items,
                pageNumber,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        /// <summary>
        /// Busca uma turma por ID
        /// </summary>
        /// <param name="id">ID da turma</param>
        /// <returns>Dados da turma</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TurmaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var turma = await _turmaService.GetByIdAsync(id);

            if (turma == null)
                return NotFound(new { message = "Turma não encontrada" });

            return Ok(turma);
        }

        /// <summary>
        /// Cadastra uma nova turma
        /// </summary>
        /// <param name="dto">Dados da turma</param>
        /// <returns>Turma cadastrada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TurmaResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] TurmaCreateDto dto)
        {
            var validator = new TurmaCreateValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

            var turma = await _turmaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = turma.Id }, turma);
        }

        /// <summary>
        /// Atualiza os dados de uma turma
        /// </summary>
        /// <param name="id">ID da turma</param>
        /// <param name="dto">Novos dados da turma</param>
        /// <returns>Turma atualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TurmaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] TurmaUpdateDto dto)
        {
            var validator = new TurmaUpdateValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

            var turma = await _turmaService.UpdateAsync(id, dto);

            if (turma == null)
                return NotFound(new { message = "Turma não encontrada" });

            return Ok(turma);
        }

        /// <summary>
        /// Exclui uma turma
        /// </summary>
        /// <param name="id">ID da turma</param>
        /// <returns>Confirmação da exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _turmaService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Turma não encontrada" });

            return NoContent();
        }
    }
}