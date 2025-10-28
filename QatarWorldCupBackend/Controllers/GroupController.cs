using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Model;
using QatarWorldCupBackend.Data;
using QatarWorldCupBackend.DTO;
using QatarWorldCupBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace QatarWorldCupBackend.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDTO>>> GetGroups()
        {
            var groupsDTO = await _groupService.GetAllGroupsAsync();
            return Ok(groupsDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDTO>> GetGroup(int id)
        {
            var groupDTO = await _groupService.GetGroupByIdAsync(id);
            if (groupDTO == null)
                return NotFound();
            return Ok(groupDTO);
        }

        [HttpPost]
        public async Task<ActionResult<GroupDTO>> CreateGroup([FromBody] GroupDTO groupDTO)
        {
            var createdGroup = await _groupService.CreateGroupAsync(groupDTO);
            return CreatedAtAction(nameof(GetGroup), new { id = createdGroup.Id }, createdGroup);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] GroupDTO groupDTO)
        {
            var success = await _groupService.UpdateGroupAsync(id, groupDTO);
            if (!success)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var success = await _groupService.DeleteGroupAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }


        [HttpPost("{groupId}/addTeam/{teamId}")]
        public async Task<IActionResult> AddTeamToGroup(int groupId, int teamId)
        {
            var success = await _groupService.AddTeamToGroupAsync(groupId, teamId);
            if (!success) return BadRequest("Unable to add team to group.");
            return Ok();
        }

        [HttpDelete("removeTeam/{teamId}")]
        public async Task<IActionResult> RemoveTeamFromGroup(int teamId)
        {
            var success = await _groupService.RemoveTeamFromGroupAsync(teamId);
            if (!success) return BadRequest("Unable to remove team from group.");
            return Ok();
        }


        [HttpDelete("deleteAllData")]
        public async Task<IActionResult> DeleteAllData()
        {
            var success = await _groupService.DeleteAllDataAsync();
            if (!success) return BadRequest("Error during the deletion of all data.");
            return NoContent(); // Successfully deleted all data
        }
    }
}
