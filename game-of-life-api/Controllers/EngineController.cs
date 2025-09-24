using FluentValidation;
using game_of_life_api.DTOs;
using game_of_life_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace game_of_life_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EngineController : ControllerBase
    {
        private readonly IGameOfLifeService _engine;
        private readonly IValidator<UploadBoardRequest> _uploadValidator;
        private readonly IValidator<AdvanceRequest> _advanceValidator;
        private readonly IValidator<FinalStateRequest> _finalValidator;

        public EngineController(
            IGameOfLifeService engine,
            IValidator<UploadBoardRequest> uploadValidator,
            IValidator<AdvanceRequest> advanceValidator,
            IValidator<FinalStateRequest> finalValidator)
        {
            _engine = engine;
            _uploadValidator = uploadValidator;
            _advanceValidator = advanceValidator;
            _finalValidator = finalValidator;
        }

        [HttpPost("next")]
        public async Task<IActionResult> Next([FromBody] UploadBoardRequest request)
        {
            var validation = await _uploadValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return BadRequest(new { Errors = validation.Errors });

            var result = _engine.ComputeNext(request.Cells);
            return Ok(result);
        }

        [HttpPost("advance")]
        public async Task<IActionResult> Advance([FromBody] AdvanceRequest request)
        {
            var validation = await _advanceValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return BadRequest(new { Errors = validation.Errors });

            var result = _engine.Advance(request.Cells, request.Steps);
            return Ok(result);
        }

        [HttpPost("final")]
        public async Task<IActionResult> Final([FromBody] FinalStateRequest request)
        {
            var validation = await _finalValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return BadRequest(new { Errors = validation.Errors });

            var result = _engine.FindFinalState(request.Cells, request.MaxAttempts);
            return Ok(new { result.Reason, result.StepsTaken, result.State });
        }
    }
}
