using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScimAPI.Utilities;
using ScimLibrary.Factories;
using ScimLibrary.Models;
using ScimLibrary.Services;

namespace ScimLibrary.Controllers
{
    [ApiController]
    [Route("scim/v2/Users")]
    public class ScimUserController : ControllerBase
    {
        private readonly ScimUserService scimUserService;
        private readonly ILogger<ScimUserController> logger;
        IScimErrorFactory scimErrorFactory;

        public ScimUserController(ScimUserService scimUserService, ILoggerFactory loggerFactory, IScimErrorFactory scimErrorFactory)
        {
            this.scimUserService = scimUserService ?? throw new ArgumentNullException(nameof(scimUserService));
            logger = loggerFactory.CreateLogger<ScimUserController>();
            this.scimErrorFactory = scimErrorFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] ScimUser user)
        {
            if (user == null)
            {
                return BadRequest(new { Message = "User data is required." });
            }

            try
            {
                await scimUserService.AddUser(user); 
                return CreatedAtAction(nameof(GetUserByIdAsync), new { id = user.ExternalId }, user); 
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
                logger?.LogError(ex, "Error creating user with username {UserName}", user?.UserName);
                return StatusCode(500, new { Message = "An unexpected error occurred while creating the user." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is required.");
            }

            try
            {
                var user = await scimUserService.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                var errorResponse = scimErrorFactory.CreateUserNotFoundError(id);
                return StatusCode(404,errorResponse);
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

        [HttpPatch("{userId}")]
        public async Task<IActionResult> PatchUser(string userId, ScimPatchOperation operations)
        {
            bool updated = await scimUserService.PatchUserAsync(userId, operations);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is required.");
            }

            var user = await scimUserService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                scimUserService.DeleteUser(user);
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
        public IActionResult GetUsersByFilter([FromQuery] int? startIndex = 1, [FromQuery] int? count = 10, [FromQuery] string filter = null)
        {
            try
            {
                var users = scimUserService.GetUserByFilter(filter);
                return Ok(users.Result);
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
