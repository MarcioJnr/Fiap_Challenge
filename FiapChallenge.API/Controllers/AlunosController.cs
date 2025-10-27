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
    public class AlunosController : ControllerBase
    {
        private readonly IAlunoService _alunoService;

        public AlunosController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        /// <summary>
        /// Lista todos os alunos com paginação
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Itens por página (padrão: 10)</param>
        /// <returns>Lista paginada de alunos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (items, totalCount) = await _alunoService.GetAllAsync(pageNumber, pageSize);

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
        /// Busca um aluno por ID
        /// </summary>
        /// <param name="id">ID do aluno</param>
        /// <returns>Dados do aluno</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AlunoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var aluno = await _alunoService.GetByIdAsync(id);

            if (aluno == null)
                return NotFound(new { message = "Aluno não encontrado" });

            return Ok(aluno);
        }

        /// <summary>
        /// Busca alunos por nome
        /// </summary>
        /// <param name="nome">Nome ou parte do nome do aluno</param>
        /// <returns>Lista de alunos encontrados</returns>
        [HttpGet("buscar/nome")]
        [ProducesResponseType(typeof(IEnumerable<AlunoResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchByName([FromQuery] string nome)
        {
            var alunos = await _alunoService.SearchByNameAsync(nome);
            return Ok(alunos);
        }

        /// <summary>
        /// Busca um aluno por CPF
        /// </summary>
        /// <param name="cpf">CPF do aluno</param>
        /// <returns>Dados do aluno</returns>
        [HttpGet("buscar/cpf")]
        [ProducesResponseType(typeof(AlunoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchByCpf([FromQuery] string cpf)
        {
            var aluno = await _alunoService.SearchByCpfAsync(cpf);

            if (aluno == null)
                return NotFound(new { message = "Aluno não encontrado" });

            return Ok(aluno);
        }

        /// <summary>
        /// Cadastra um novo aluno
        /// </summary>
        /// <param name="dto">Dados do aluno</param>
        /// <returns>Aluno cadastrado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AlunoResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] AlunoCreateDto dto)
        {
            var validator = new AlunoCreateValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

            try
            {
                var aluno = await _alunoService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = aluno.Id }, aluno);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza os dados de um aluno
        /// </summary>
        /// <param name="id">ID do aluno</param>
        /// <param name="dto">Novos dados do aluno</param>
        /// <returns>Aluno atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AlunoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] AlunoUpdateDto dto)
        {
            var validator = new AlunoUpdateValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });

            try
            {
                var aluno = await _alunoService.UpdateAsync(id, dto);

                if (aluno == null)
                    return NotFound(new { message = "Aluno não encontrado" });

                return Ok(aluno);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Exclui um aluno
        /// </summary>
        /// <param name="id">ID do aluno</param>
        /// <returns>Confirmação da exclusão</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _alunoService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Aluno não encontrado" });

            return NoContent();
        }
    }
}