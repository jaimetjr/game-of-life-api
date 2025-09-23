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

        public EngineController(IGameOfLifeService engine)
        {
            _engine = engine;
        }

        [HttpPost("next")]
        public IActionResult Next([FromBody] bool[][] cells)
        {
            var result = _engine.ComputeNext(cells);
            return Ok(result);
        }

        [HttpPost("advance")]
        public IActionResult Advance([FromBody] bool[][] cells, [FromQuery] int steps = 1)
        {
            if (steps < 0) return BadRequest("Steps must be >= 0");
            var result = _engine.Advance(cells, steps);
            return Ok(result);
        }

        [HttpPost("final")]
        public IActionResult Final([FromBody] bool[][] cells, [FromQuery] int maxAttempts = 1000)
        {
            if (maxAttempts <= 0) return BadRequest("maxAttempts must be > 0");
            var result = _engine.FindFinalState(cells, maxAttempts);
            return Ok(new
            {
                result.Reason,
                result.StepsTaken,
                result.State
            });
        }
    }
}
