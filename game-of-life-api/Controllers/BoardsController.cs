using FluentValidation;
using game_of_life_api.Data;
using game_of_life_api.DTOs;
using game_of_life_api.Helpers.Enum;
using game_of_life_api.Models;
using game_of_life_api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace game_of_life_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly IBoardRepository _boardRepository;
        private readonly IValidator<UploadBoardRequest> _validator;
        private readonly IGameOfLifeService _engine;

        public BoardsController(IBoardRepository boardRepository, IGameOfLifeService engine, IValidator<UploadBoardRequest> validator)
        {
            _boardRepository = boardRepository;
            _validator = validator;
            _engine = engine;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromBody] UploadBoardRequest request, CancellationToken ct)
        {
            // Validate input
            var validation = await _validator.ValidateAsync(request, ct);
            if (!validation.IsValid)
            {
                return BadRequest(new
                {
                    Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            // Map DTO -> Entity
            var entity = new Board
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Rows = request.Rows,
                Cols = request.Cols,
                CellsJson = JsonSerializer.Serialize(request.Cells),
                CreatedAt = DateTime.UtcNow
            };

            var id = await _boardRepository.AddAsync(entity, ct);

            return Ok(new { id });
        }

        [HttpPost("{id:guid}/next")]
        public async Task<IActionResult> Next(Guid id, CancellationToken ct)
        {
            var board = await _boardRepository.GetByIdAsync(id, ct);
            if (board is null)
                return NotFound(new { message = $"Board {id} not found" });

            var cells = JsonSerializer.Deserialize<bool[][]>(board.CellsJson) ?? Array.Empty<bool[]>();
            var next = _engine.ComputeNext(cells);

            return Ok(new
            {
                id = board.Id,
                rows = board.Rows,
                cols = board.Cols,
                state = next
            });
        }

        [HttpPost("{id:guid}/advance")]
        public async Task<IActionResult> Advance(Guid id, [FromQuery] int steps, CancellationToken ct)
        {
            if (steps <= 0 || steps > 10_000)
                return BadRequest(new { message = $"Steps must be > 0 and <= 10000. Provided: {steps}" });

            var board = await _boardRepository.GetByIdAsync(id, ct);
            if (board is null)
                return NotFound(new { message = $"Board {id} not found" });

            var cells = JsonSerializer.Deserialize<bool[][]>(board.CellsJson) ?? Array.Empty<bool[]>();
            var advanced = _engine.Advance(cells, steps);

            return Ok(new { id = board.Id, rows = board.Rows, cols = board.Cols, state = advanced });
        }

        [HttpPost("{id:guid}/final")]
        public async Task<IActionResult> Final(Guid id, [FromQuery] int maxAttempts = 1000, CancellationToken ct = default)
        {
            if (maxAttempts <= 0 || maxAttempts > 50_000)
                return BadRequest(new { message = $"maxAttempts must be > 0 and <= 50000. Provided: {maxAttempts}" });

            var board = await _boardRepository.GetByIdAsync(id, ct);
            if (board is null)
                return NotFound(new { message = $"Board {id} not found" });

            var cells = JsonSerializer.Deserialize<bool[][]>(board.CellsJson) ?? Array.Empty<bool[]>();
            var result = _engine.FindFinalState(cells, maxAttempts);

            return result.Reason switch
            {
                TerminationReason.Stable or TerminationReason.Extinct =>
                    Ok(new { id = board.Id, rows = board.Rows, cols = board.Cols, state = result.State, reason = result.Reason }),
                TerminationReason.Loop or TerminationReason.Unresolved =>
                    UnprocessableEntity(new { id = board.Id, reason = result.Reason, stepsTaken = result.StepsTaken }),
                _ => StatusCode(500, new { message = "Unexpected termination reason." })
            };
        }
    }
}
