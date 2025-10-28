using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QatarWorldCupBackend.Data;
using QatarWorldCupBackend.DTO;
using QatarWorldCupBackend.Interfaces;
using QatarWorldCupBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QatarWorldCupBackend.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeams()
        {
            var teamsDTO = await _teamService.GetAllTeamsAsync();
            return Ok(teamsDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDTO>> GetTeam(int id)
        {
            var teamDTO = await _teamService.GetTeamByIdAsync(id);
            if (teamDTO == null)
                return NotFound();
            return Ok(teamDTO);
        }

        [HttpPost]
        public async Task<ActionResult<TeamDTO>> CreateTeam([FromBody] TeamDTO teamDTO)
        {
            try
            {
                var createdTeam = await _teamService.CreateTeamAsync(teamDTO);
                return CreatedAtAction(nameof(GetTeam), new { id = createdTeam.Id }, createdTeam);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] TeamDTO teamDTO)
        {
            var success = await _teamService.UpdateTeamAsync(id, teamDTO);
            if (!success)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var success = await _teamService.DeleteTeamAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }

        [HttpPost("{teamId}/addToGroup/{groupId}")]
        public async Task<IActionResult> AddTeamToGroup(int teamId, int groupId)
        {
            var success = await _teamService.AddTeamToGroupAsync(teamId, groupId);
            if (!success)
                return BadRequest("Unable to add team to group.");
            return Ok();
        }

        [HttpPost("{teamId}/removeFromGroup")]
        public async Task<IActionResult> RemoveTeamFromGroup(int teamId)
        {
            var success = await _teamService.RemoveTeamFromGroupAsync(teamId);
            if (!success)
                return BadRequest("Unable to remove team from group.");
            return Ok();
        }

        [HttpGet("byGroup/{groupId}")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> GetTeamByGroupId(int groupId)
        {
            var teams = await _teamService.GetTeamByGroupIdAsync(groupId);
            if (teams == null)
            {
                return NotFound();
            }
            return Ok(teams);
        }

        // DELETE: api/Match/DeleteAll
        [HttpDelete("DeleteAll")]
        public async Task<IActionResult> DeleteAllTeams()
        {
            var result = await _teamService.DeleteAllTeamsAsync();
            if (!result)
            {
                return NotFound("No teams found to delete.");
            }
            return NoContent();
        }


    }
}
