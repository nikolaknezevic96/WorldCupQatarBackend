using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QatarWorldCupBackend.DTO;
using QatarWorldCupBackend.Interfaces; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QatarWorldCupBackend.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class StadiumController : ControllerBase
    {
        private readonly IStadiumService _stadiumService;

        public StadiumController(IStadiumService stadiumService)
        {
            _stadiumService = stadiumService;
        }

        // GET: api/Stadium
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StadiumDTO>>> GetStadiums()
        {
            var stadiumsDTO = await _stadiumService.GetAllStadiumsAsync();
            return Ok(stadiumsDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StadiumDTO>> GetStadium(int id)
        {
            var stadiumDTO = await _stadiumService.GetStadiumByIdAsync(id);
            if (stadiumDTO == null)
                return NotFound();
            return Ok(stadiumDTO);
        }

        [HttpPost]
        public async Task<ActionResult<StadiumDTO>> CreateStadium([FromBody] StadiumDTO stadiumDTO)
        {
            var createdStadium = await _stadiumService.CreateStadiumAsync(stadiumDTO);
            return CreatedAtAction(nameof(GetStadium), new { id = createdStadium.Id }, createdStadium);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStadium(int id, [FromBody] StadiumDTO stadiumDTO)
        {
            var success = await _stadiumService.UpdateStadiumAsync(id, stadiumDTO);
            if (!success)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStadium(int id)
        {
            var success = await _stadiumService.DeleteStadiumAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}
