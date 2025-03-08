using Microsoft.AspNetCore.Mvc;
using ScimAPI.Utilities;
using ScimLibrary.Factories;
using ScimLibrary.Models;
using ScimLibrary.Services;

namespace ScimLibrary.Controllers
{

    [ApiController]
    [Route("scim/v2/Groups")]
    public class GroupController : ControllerBase
    {
        private readonly ScimGroupService scimGroupService;
        private readonly ILogger<GroupController> logger;
        IScimErrorFactory scimErrorFactory;

        public GroupController(ScimGroupService scimGroupService, ILoggerFactory loggerFactory, IScimErrorFactory scimErrorFactory)
        {
            this.scimGroupService = scimGroupService ?? throw new ArgumentNullException(nameof(scimGroupService));
            logger = loggerFactory.CreateLogger<GroupController>();
            this.scimErrorFactory = scimErrorFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupAsync([FromBody] ScimGroup group)
        {
            if (group == null)
            {
                return BadRequest(new { Message = "User data is required." });
            }

            try
            {
                await scimGroupService.AddGroup(group);
                return CreatedAtAction(nameof(GetGroupByIdAsync), new { id = group.ExternalId }, group);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = "Invalid user data.", Details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = "A conflict occurred while creating the user.", Details = ex.Message });
            }
            catch (ApplicationException ex)
            {
                logger?.LogError(ex, "Error creating user with username {UserName}", group?.DisplayName);
                return StatusCode(500, new { Message = "An unexpected error occurred while creating the user." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is required.");
            }

            try
            {
                var group = await scimGroupService.GetGroupById(id);
                if (group == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                return Ok(group);
            }
            catch (KeyNotFoundException)
            {
                var errorResponse = scimErrorFactory.CreateGroupNotFoundError(id);
                return StatusCode(404, errorResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = "Invalid user ID format.", Details = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = "A conflict occurred while retrieving the user.", Details = ex.Message });
            }
            catch (ApplicationException ex)
            {
                logger?.LogError(ex, "Error retrieving user with ID {UserId}", id);
                return StatusCode(500, new { Message = "An unexpected error occurred while retrieving the user." });
            }
        }

        [HttpPatch("{groupId}")]
        public async Task<IActionResult> PatchGroupAsync(string groupId, ScimPatchOperation operations)
        {
            bool updated = await scimGroupService.PatchGroupAsync(groupId, operations);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Group ID is required.");
            }

            var group = await scimGroupService.GetGroupById(id);
            if (group == null)
            {
                return NotFound();
            }

            try
            {
                scimGroupService.DeleteGroup(group);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "User not found or already deleted." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = "Invalid operation: " + ex.Message });
            }
            catch (ApplicationException ex)
            {
                logger?.LogError(ex, "Error deleting user with ID {UserId}", id);
                return StatusCode(500, new { Message = "An unexpected error occurred while deleting the user." });
            }
        }

        [HttpGet]
        public IActionResult GetGroupByFilter([FromQuery] string filter, [FromQuery] int? startIndex = 1, [FromQuery] int? count = 10)
        {
            try
            {
                var group = scimGroupService.GetGroupByFilter(filter);
                return Ok(group);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogWarning(ex, "Users not found.");
                return NotFound(new { message = "Users not found." });
            }
            catch (ApplicationException ex)
            {
                logger.LogError(ex, "An unexpected error occurred while retrieving users.");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
