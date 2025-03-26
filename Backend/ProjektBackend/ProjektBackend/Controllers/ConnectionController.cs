using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ProjektBackend.Models;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Connection = ProjektBackend.Models.Connection;

#pragma warning disable CS0168

namespace ProjektBackend.Controllers
{
    [Route("api/connections")]
    [ApiController]
    public class ConnectionController : ControllerBase
    {
        private readonly ProjektContext _context;

        public ConnectionController(ProjektContext context)
        {
            _context = context;
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("fetchConnections")]
        public async Task<ActionResult<Connection>> FetchConnections()
        {
            try
            {
                var connections = await _context.Connections.ToListAsync();

                if (connections != null && connections.Any())
                {
                    return StatusCode(200, connections);
                }
                return StatusCode(404, "There are currently no Connections.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching connections.");
            }
        }

        [Authorize(Policy = "EmployerSelfOrAdmin")]
        [HttpGet("fetchConnection")]
        public async Task<ActionResult<IEnumerable<Connection>>> FetchConnections(int ReceiverId, int? userId = null)
        {
            try
            {
                int targetUserId;
                bool isAdmin = User.IsInRole("Admin");

                if (userId.HasValue && isAdmin)
                {
                    targetUserId = userId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                        return StatusCode(401, "User ID not found in token.");

                    targetUserId = int.Parse(userIdClaim.Value);
                }

                if (ReceiverId != targetUserId)
                {
                    return StatusCode(403, "You dont have access to this connection.");
                }

                var connection = await _context.Connections.Where(x => x.ReceiverId == ReceiverId).ToListAsync();

                if (connection != null && connection.Any())
                {
                    return StatusCode(200, connection);
                }
                return StatusCode(404, "No Connection can be found with this Id.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching connections.");
            }
        }

        [Authorize(Policy = "EmployeeSelfOnly")]
        [HttpPost("postNewConnection")]
        public async Task<ActionResult> PostNewConnection(CreateConnectionDto createConnectionDto, int? userId = null)
        {
            try
            {
                int targetUserId;
                bool isAdmin = User.IsInRole("Admin");

                if (userId.HasValue && isAdmin)
                {
                    targetUserId = userId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                        return StatusCode(401, "User ID not found in token.");

                    targetUserId = int.Parse(userIdClaim.Value);
                }

                var newConnection = new Connection
                {
                    RequesterId = targetUserId,
                    ReceiverId = createConnectionDto.ReceiverId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                };
                if (newConnection != null)
                {
                    _context.Add(newConnection);
                    await _context.SaveChangesAsync();
                    return StatusCode(201, "Request created successfully.");
                }
                return StatusCode(400, "Invalid data.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to create connection. It may already exist.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the connection.");
            }
        }

        [Authorize(Policy = "EmployerSelfOnly")]
        [HttpPut("editConnection/{ReceiverId}")]
        public async Task<ActionResult> UpdateConnection(int ReceiverId, int RequesterId, UpdateConnectionDto updateConnectionDto, int? userId = null)
        {
            try
            {
                int targetUserId;
                bool isAdmin = User.IsInRole("Admin");

                if (userId.HasValue && isAdmin)
                {
                    targetUserId = userId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                        return StatusCode(401, "User ID not found in token.");

                    targetUserId = int.Parse(userIdClaim.Value);
                }

                var existingConnection = await _context.Connections.FirstOrDefaultAsync(x => x.ReceiverId == ReceiverId && x.RequesterId == RequesterId);

                if (existingConnection != null)
                {
                    if (updateConnectionDto.Status != "Accepted" && updateConnectionDto.Status != "Declined")
                    {
                        return StatusCode(403, "You are not allowed to give custom Status'.");
                    }
                    existingConnection.Status = updateConnectionDto.Status;

                    _context.Update(existingConnection);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, $"Request status has been changed to: {existingConnection.Status}");
                }
                return StatusCode(404, "No Connection can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to update connection.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the connection.");
            }
        }

        [Authorize(Policy = "SelfOrAdmin")]
        [HttpDelete("deleteConnection/{RequesterId}/{ReceiverId}")]
        public async Task<ActionResult> DeleteConnection(int ReceiverId, int RequesterId, int? userId = null)
        {
            try
            {
                int targetUserId;
                bool isAdmin = User.IsInRole("Admin");

                if (userId.HasValue && isAdmin)
                {
                    targetUserId = userId.Value;
                }
                else
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim == null)
                        return StatusCode(401, "User ID not found in token.");

                    targetUserId = int.Parse(userIdClaim.Value);
                }

                var connection = await _context.Connections.FirstOrDefaultAsync(x => x.ReceiverId == ReceiverId && x.RequesterId == RequesterId);

                if (connection != null)
                {
                    _context.Remove(connection);
                    await _context.SaveChangesAsync();
                    return StatusCode(200, "Connection successfully deleted.");
                }
                return StatusCode(404, "No Connection can be found with this Id.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(409, "Unable to delete connection.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the connection.");
            }
        }
    }
}
