using Domain.Model;
using QatarWorldCupBackend.DTO;
using QatarWorldCupBackend.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace QatarWorldCupBackend.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        // GET: api/Match
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatches()
        {
            var matchDTOs = await _matchService.GetAllMatchesAsync();
            return Ok(matchDTOs);
        }

        // GET: api/Match/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDTO>> GetMatch(int id)
        {
            var matchDTO = await _matchService.GetMatchByIdAsync(id);
            if (matchDTO == null)
            {
                return NotFound();
            }
            return Ok(matchDTO);
        }

        // POST: api/Match
        [HttpPost]
        public async Task<IActionResult> PostMatch([FromBody] MatchDTO matchDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdMatch = await _matchService.CreateMatchAsync(matchDTO);
                return CreatedAtAction(nameof(GetMatch), new { id = createdMatch.Id }, createdMatch);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // PUT: api/Match/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatch(int id, MatchDTO matchDTO)
        {
            if (!await _matchService.UpdateMatchAsync(id, matchDTO))
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/Match/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            if (!await _matchService.DeleteMatchAsync(id))
            {
                return NotFound();
            }
            return NoContent();
        }


        // PUT: api/Match/5/SetScore
        [HttpPut("{id}/SetScore")]
        public async Task<IActionResult> SetMatchScore(int id, [FromBody] ScoreDTO scoreDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updateResult = await _matchService.SetMatchResultAsync(id, scoreDTO.Team1GoalsScored, scoreDTO.Team2GoalsScored);
            if (!updateResult)
            {
                return NotFound(new { Message = "Match not found or update unsuccessfull." });
            }

            return NoContent(); // Successfully updated response
        }


        [HttpGet("ByGroup/{groupId}")]
        public async Task<IActionResult> GetMatchesByGroupId(int groupId)
        {
            var matches = await _matchService.GetMatchesByGroupIdAsync(groupId);
            if (matches == null || !matches.Any())
                return NotFound("No matches found for the given group.");

            return Ok(matches);
        }

        // DELETE: api/Match/DeleteAll
        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteAllMatches()
        {
            var result = await _matchService.DeleteAllMatchesAsync();
            if (!result)
            {
                return NotFound("No matches found to delete.");
            }
            return NoContent();
        }


    }
}
