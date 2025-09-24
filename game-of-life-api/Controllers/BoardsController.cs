using FluentValidation;
using game_of_life_api.Data;
using game_of_life_api.DTOs;
using game_of_life_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace game_of_life_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly IBoardRepository _boardRepository;
        private readonly IValidator<UploadBoardRequest> _validator;

        public BoardsController(IBoardRepository boardRepository, IValidator<UploadBoardRequest> validator)
        {
            _boardRepository = boardRepository;
            _validator = validator;
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
                CellsJson = System.Text.Json.JsonSerializer.Serialize(request.Cells),
                CreatedAt = DateTime.UtcNow
            };

            var id = await _boardRepository.AddAsync(entity, ct);

            return Ok(new { id });
        }
    }
}
